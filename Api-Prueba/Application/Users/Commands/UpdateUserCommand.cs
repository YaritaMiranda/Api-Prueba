using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Users;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Users.Commands;

public sealed record UpdateUserCommand(int Id, string Name, string Email, bool IsActive);

public sealed class UpdateUserCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<UserSummaryResponse>> HandleAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await dbContext.Users.FindAsync([command.Id], cancellationToken);

        if (user is null)
        {
            return OperationResult<UserSummaryResponse>.NotFound("Usuario no encontrado.");
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var emailExists = await dbContext.Users
            .AnyAsync(otherUser => otherUser.Id != command.Id && otherUser.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return OperationResult<UserSummaryResponse>.Conflict("Ya existe otro usuario con ese email.");
        }

        user.Name = command.Name.Trim();
        user.Email = normalizedEmail;
        user.IsActive = command.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<UserSummaryResponse>.Success(
            new UserSummaryResponse(user.Id, user.Name, user.Email, user.IsActive));
    }
}
