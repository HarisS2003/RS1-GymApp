using Market.Shared.Validation;

namespace Market.Application.Modules.Catalog.Users.Commands.Create;

public class CreateUserCommandHandler(IAppDbContext ctx, IPasswordHasher<UserEntity> hasher)
    : IRequestHandler<CreateUserCommand, string>
{
    public async Task<string> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var normalizedFirst = request.FirstName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedFirst)) throw new ValidationException("First name is required.");

        var normalizedLast = request.LastName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedLast)) throw new ValidationException("Last name is required.");

        var normalizedEmail = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail)) throw new ValidationException("Email is required.");

        if (!BosnianPhoneNumberValidator.IsValid(request.PhoneNumber, out var normalizedPhone))
            throw new ValidationException("Invalid Bosnian phone number format.");

        var normalizedPassword = request.Password.Trim();
        if (string.IsNullOrWhiteSpace(normalizedPassword)) throw new ValidationException("Password is required.");

        if (!await ctx.Roles.AnyAsync(x => x.Id == request.RoleId, ct))
            throw new ValidationException("Invalid RoleId.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId, ct))
            throw new ValidationException("Invalid GymId.");

        var emailLower = normalizedEmail.ToLower();
        var exists = await ctx.Users.AnyAsync(
            x => x.Email.ToLower() == emailLower,
            ct);
        if (exists) throw new MarketConflictException("User with this email already exists.");

        var user = new UserEntity
        {
            FirstName = normalizedFirst,
            LastName = normalizedLast,
            Email = normalizedEmail,
            PhoneNumber = normalizedPhone!,
            RoleId = request.RoleId,
            GymId = request.GymId
        };

        user.PasswordHash = hasher.HashPassword(user, normalizedPassword);

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync(ct);
        return user.PublicId;
    }
}
