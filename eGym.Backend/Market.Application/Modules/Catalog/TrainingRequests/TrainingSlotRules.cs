namespace Market.Application.Modules.Catalog.TrainingRequests;

internal static class TrainingSlotRules
{
    public static readonly TimeSpan DayStart = new(8, 0, 0);
    public static readonly TimeSpan DayEnd = new(21, 0, 0);
    public const int SlotMinutes = 30;

    public static readonly TimeSpan ShiftOnApprove = TimeSpan.FromHours(1);

    public static IEnumerable<TimeSpan> GenerateSlots()
    {
        for (var t = DayStart; t < DayEnd; t = t.Add(TimeSpan.FromMinutes(SlotMinutes)))
            yield return t;
    }

    public static bool BlocksSlot(TrainingRequestStatus status) =>
        status is TrainingRequestStatus.Pending or TrainingRequestStatus.Approved;

    public static TimeSpan NormalizeTime(TimeSpan time) =>
        new(time.Hours, time.Minutes, 0);

    public static (DateTime Date, TimeSpan StartTime) ShiftSession(DateTime date, TimeSpan startTime)
    {
        var normalized = NormalizeTime(startTime);
        var shiftedTime = normalized.Add(ShiftOnApprove);
        if (shiftedTime < DayEnd)
            return (date, shiftedTime);

        var overflow = shiftedTime - DayEnd;
        return (date.AddDays(1), DayStart.Add(overflow));
    }

    internal readonly record struct SessionBlock(TimeSpan Start, bool IsApproved);

    /// <summary>
    /// Rentable 30-min starts for a day. Approved sessions block 1h before/after; conflicts omitted.
    /// </summary>
    internal static List<TimeSpan> BuildRentableSlots(IReadOnlyList<SessionBlock> sessions)
    {
        var blocked = BuildBlockedIntervals(sessions);
        var rentable = new List<TimeSpan>();
        var cursor = DayStart;
        var slotLength = TimeSpan.FromMinutes(SlotMinutes);

        while (cursor.Add(slotLength) <= DayEnd)
        {
            var slotEnd = cursor.Add(slotLength);

            if (!Overlaps(cursor, slotEnd, blocked))
                rentable.Add(cursor);

            cursor = cursor.Add(slotLength);

            // Dynamic +1h gap after each approved session (before and after buffer already in blocked)
            foreach (var session in sessions.Where(s => s.IsApproved))
            {
                var sessionEnd = session.Start.Add(slotLength);
                var gapEnd = sessionEnd.Add(ShiftOnApprove);
                if (cursor > sessionEnd && cursor < gapEnd)
                    cursor = gapEnd;
            }
        }

        return rentable;
    }

    internal static bool IsRentableSlot(IReadOnlyList<SessionBlock> sessions, TimeSpan startTime)
    {
        var normalized = NormalizeTime(startTime);
        return BuildRentableSlots(sessions).Any(slot => slot == normalized);
    }

    private static List<(TimeSpan From, TimeSpan To)> BuildBlockedIntervals(IReadOnlyList<SessionBlock> sessions)
    {
        var slotLength = TimeSpan.FromMinutes(SlotMinutes);
        var raw = sessions
            .Select(s =>
            {
                var start = NormalizeTime(s.Start);
                var end = start.Add(slotLength);
                if (!s.IsApproved)
                    return (From: start, To: end);

                return (
                    From: start.Subtract(ShiftOnApprove),
                    To: end.Add(ShiftOnApprove));
            })
            .OrderBy(x => x.From)
            .ToList();

        return MergeIntervals(raw);
    }

    private static List<(TimeSpan From, TimeSpan To)> MergeIntervals(
        IReadOnlyList<(TimeSpan From, TimeSpan To)> intervals)
    {
        if (intervals.Count == 0)
            return [];

        var merged = new List<(TimeSpan From, TimeSpan To)> { intervals[0] };

        for (var i = 1; i < intervals.Count; i++)
        {
            var current = intervals[i];
            var last = merged[^1];

            if (current.From <= last.To)
                merged[^1] = (last.From, current.To > last.To ? current.To : last.To);
            else
                merged.Add(current);
        }

        return merged;
    }

    private static bool Overlaps(
        TimeSpan slotStart,
        TimeSpan slotEnd,
        IReadOnlyList<(TimeSpan From, TimeSpan To)> blocked) =>
        blocked.Any(b => slotStart < b.To && slotEnd > b.From);
}
