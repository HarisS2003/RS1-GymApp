namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

public sealed class GetTrainerAvailableSlotsQuery : IRequest<List<TrainerAvailableSlotDto>>
{
    public string TrainerPublicId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
