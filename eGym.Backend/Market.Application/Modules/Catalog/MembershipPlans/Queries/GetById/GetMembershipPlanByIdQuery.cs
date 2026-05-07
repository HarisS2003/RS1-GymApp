namespace Market.Application.Modules.Catalog.MembershipPlans.Queries.GetById;

public class GetMembershipPlanByIdQuery : IRequest<GetMembershipPlanByIdQueryDto>
{
    public int Id { get; set; }
}
