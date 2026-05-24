namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

public sealed class TrainerAvailableSlotDto
{
    public string StartTime { get; set; } = "";
    public bool IsAvailable { get; set; }
}
