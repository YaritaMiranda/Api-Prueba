using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Users.Commands;

public sealed record DeleteUserCommand(int Id);

public sealed class DeleteUserCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<bool>> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await dbContext.Users.FindAsync([command.Id], cancellationToken);

        if (user is null)
        {
            return OperationResult<bool>.NotFound("Usuario no encontrado.");
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<bool>.Success(true);
    }
}
