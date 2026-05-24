namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;

public sealed class GetTrainerAvailableSlotsQuery : IRequest<List<TrainerAvailableSlotDto>>
{
    public int TrainerId { get; set; }
    public DateTime Date { get; set; }
}
