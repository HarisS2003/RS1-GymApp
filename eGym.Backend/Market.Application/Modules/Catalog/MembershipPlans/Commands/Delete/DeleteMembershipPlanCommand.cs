namespace Market.Application.Modules.Catalog.MembershipPlans.Commands.Delete;

public class DeleteMembershipPlanCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
