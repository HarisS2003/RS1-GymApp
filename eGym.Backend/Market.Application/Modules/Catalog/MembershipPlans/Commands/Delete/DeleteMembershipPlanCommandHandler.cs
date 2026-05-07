namespace Market.Application.Modules.Catalog.MembershipPlans.Commands.Delete;

public class DeleteMembershipPlanCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<DeleteMembershipPlanCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        if (!appCurrentUser.IsAdmin)
            throw new MarketBusinessRuleException("123", "Samo admin moze brisati.");

        var entity = await context.MembershipPlans
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new MarketNotFoundException("Plan članstva nije pronađen.");

        context.MembershipPlans.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
