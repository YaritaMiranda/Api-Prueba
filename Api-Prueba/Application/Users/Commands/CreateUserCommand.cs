using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Users;
using UserCurrencyApi.Domain.Entities;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Users.Commands;

public sealed record CreateUserCommand(string Name, string Email);

public sealed class CreateUserCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<UserSummaryResponse>> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return OperationResult<UserSummaryResponse>.Conflict("Ya existe un usuario con ese email.");
        }

        var user = new User
        {
            Name = command.Name.Trim(),
            Email = normalizedEmail,
            IsActive = true
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<UserSummaryResponse>.Success(
            new UserSummaryResponse(user.Id, user.Name, user.Email, user.IsActive));
    }
}
