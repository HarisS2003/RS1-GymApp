using System.Text.Json;

namespace Market.Application.Modules.Identity.UserMemberships.Services;

public sealed class MembershipReplayState
{
    public bool Exists { get; set; }
    public int UserId { get; set; }
    public int MembershipPlanId { get; set; }
    public string? PlanName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "None";
    public bool IsFrozen { get; set; }
}

public static class MembershipEventReplayer
{
    public static MembershipReplayState Replay(IEnumerable<MembershipEvent> events, DateTime asOfDate)
    {
        var asOfCalendarDate = MembershipHistoryDateHelper.AsOfCalendarDate(asOfDate);
        var state = new MembershipReplayState();

        foreach (var evt in events.OrderBy(e => e.CreatedAt).ThenBy(e => e.Id))
        {
            if (evt.CreatedAt.Date > asOfCalendarDate)
            {
                break;
            }

            Apply(state, evt);
        }

        if (!state.Exists)
        {
            state.Status = "None";
            return state;
        }

        FinalizeStatus(state, asOfCalendarDate);
        return state;
    }

    public static MembershipReplayState FromMembershipSnapshot(
        int userId,
        int membershipPlanId,
        string? planName,
        DateTime startDate,
        DateTime endDate,
        DateTime asOfDate)
    {
        var asOfCalendarDate = MembershipHistoryDateHelper.AsOfCalendarDate(asOfDate);

        if (startDate.Date > asOfCalendarDate || endDate.Date < asOfCalendarDate)
        {
            return new MembershipReplayState { Status = "None" };
        }

        var state = new MembershipReplayState
        {
            Exists = true,
            UserId = userId,
            MembershipPlanId = membershipPlanId,
            PlanName = planName,
            StartDate = startDate,
            EndDate = endDate,
            IsFrozen = false,
            Status = "Active",
        };

        FinalizeStatus(state, asOfCalendarDate);
        return state;
    }

    private static void FinalizeStatus(MembershipReplayState state, DateTime asOfCalendarDate)
    {
        if (!state.Exists)
        {
            state.Status = "None";
            return;
        }

        if (state.EndDate.HasValue && state.EndDate.Value.Date < asOfCalendarDate)
        {
            state.Status = "Expired";
            state.IsFrozen = false;
            return;
        }

        if (!state.IsFrozen)
        {
            state.Status = "Active";
        }
    }

    private static void Apply(MembershipReplayState state, MembershipEvent evt)
    {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(evt.EventData) ? "{}" : evt.EventData);
        var root = doc.RootElement;

        switch (evt.EventType)
        {
            case MembershipEventTypes.Created:
            case "MembershipPurchased":
                state.Exists = true;
                state.UserId = TryGetInt(root, "userId");
                state.MembershipPlanId = TryGetInt(root, "membershipPlanId");
                state.PlanName = TryGetString(root, "planName");
                state.StartDate = TryGetDate(root, "startDate");
                state.EndDate = TryGetDate(root, "endDate");
                state.IsFrozen = false;
                state.Status = "Active";
                break;

            case MembershipEventTypes.Frozen:
                state.IsFrozen = true;
                state.Status = "Frozen";
                break;

            case MembershipEventTypes.Activated:
                state.IsFrozen = false;
                state.Status = state.EndDate.HasValue && state.EndDate.Value.Date >= evt.CreatedAt.Date
                    ? "Active"
                    : "Expired";
                break;

            case MembershipEventTypes.EndDateAdjusted:
                state.EndDate = TryGetDate(root, "endDate");
                if (!state.IsFrozen && state.EndDate.HasValue)
                {
                    state.Status = state.EndDate.Value.Date >= evt.CreatedAt.Date ? "Active" : "Expired";
                }
                break;
        }
    }

    private static int TryGetInt(JsonElement root, string property)
    {
        if (TryGetProperty(root, property, out var value) && value.ValueKind == JsonValueKind.Number)
        {
            return value.GetInt32();
        }

        return 0;
    }

    private static string? TryGetString(JsonElement root, string property)
    {
        if (TryGetProperty(root, property, out var value) && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return null;
    }

    private static DateTime? TryGetDate(JsonElement root, string property)
    {
        if (!TryGetProperty(root, property, out var value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            var raw = value.GetString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            if (DateTime.TryParse(raw, out var parsed) && parsed.Year > 1)
            {
                return parsed;
            }

            return null;
        }

        if (value.ValueKind == JsonValueKind.Number)
        {
            try
            {
                var dt = value.GetDateTime();
                return dt.Year > 1 ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    private static bool TryGetProperty(JsonElement root, string property, out JsonElement value)
    {
        if (root.TryGetProperty(property, out value))
        {
            return true;
        }

        foreach (var prop in root.EnumerateObject())
        {
            if (string.Equals(prop.Name, property, StringComparison.OrdinalIgnoreCase))
            {
                value = prop.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}
