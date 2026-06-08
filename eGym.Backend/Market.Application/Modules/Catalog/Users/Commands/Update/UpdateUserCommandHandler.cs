using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Users.Commands.Update;

public sealed class UpdateUserCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser,
    IPasswordHasher<UserEntity> hasher)
    : IRequestHandler<UpdateUserCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var entity = await ctx.Users.FirstOrDefaultAsync(
            x => x.PublicId == request.PublicId && x.GymId == currentGymId,
            ct);
        if (entity is null) throw new MarketNotFoundException("User not found.");

        var normalizedFirst = request.FirstName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedFirst)) throw new ValidationException("First name is required.");

        var normalizedLast = request.LastName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedLast)) throw new ValidationException("Last name is required.");

        var normalizedEmail = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail)) throw new ValidationException("Email is required.");

        if (!BosnianPhoneNumberValidator.IsValid(request.PhoneNumber, out var normalizedPhone))
            throw new ValidationException("Invalid Bosnian phone number format.");

        if (!await ctx.Roles.AnyAsync(x => x.Id == request.RoleId, ct))
            throw new ValidationException("Invalid RoleId.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId, ct))
            throw new ValidationException("Invalid GymId.");

        var emailLower = normalizedEmail.ToLower();
        var exists = await ctx.Users.AnyAsync(
            x => x.Id != entity.Id && x.Email.ToLower() == emailLower,
            ct);
        if (exists) throw new MarketConflictException("User with this email already exists.");

        entity.FirstName = normalizedFirst;
        entity.LastName = normalizedLast;
        entity.Email = normalizedEmail;
        entity.PhoneNumber = normalizedPhone!;
        entity.RoleId = request.RoleId;
        entity.GymId = request.GymId;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var pwd = request.Password.Trim();
            if (string.IsNullOrWhiteSpace(pwd)) throw new ValidationException("Password cannot be whitespace-only.");
            entity.PasswordHash = hasher.HashPassword(entity, pwd);
        }

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private async Task<int> GetCurrentGymIdAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var gymId = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.GymId)
            .FirstOrDefaultAsync(ct);

        return gymId == 0
            ? throw new MarketNotFoundException("User not found.")
            : gymId;
    }
}
