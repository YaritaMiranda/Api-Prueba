using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Users;
using UserCurrencyApi.Domain.Entities;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Users.Commands;

public sealed record BulkCreateUsersCommand(IReadOnlyList<CreateUserRequest> Users);

public sealed class BulkCreateUsersCommandHandler(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IValidator<CreateUserRequest> userValidator)
{
    public async Task<BulkCreateUsersResponse> HandleAsync(
        BulkCreateUsersCommand command,
        CancellationToken cancellationToken)
    {
        var tasks = command.Users
            .Select((user, index) => CreateUserAsync(index, user, cancellationToken))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        return new BulkCreateUsersResponse(
            results.Count(result => result.CreatedUser is not null),
            results.Where(result => result.CreatedUser is not null)
                .Select(result => result.CreatedUser!)
                .ToArray(),
            results.Where(result => result.Failure is not null)
                .Select(result => result.Failure!)
                .ToArray());
    }

    private async Task<BulkCreateUserResult> CreateUserAsync(
        int index,
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await userValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BulkCreateUserResult.Failed(
                index,
                request.Email,
                validationResult.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return BulkCreateUserResult.Failed(index, request.Email, ["Ya existe un usuario con ese email."]);
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            IsActive = true
        };

        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            return BulkCreateUserResult.Failed(index, request.Email, [ex.GetBaseException().Message]);
        }

        return BulkCreateUserResult.Created(new UserSummaryResponse(user.Id, user.Name, user.Email, user.IsActive));
    }

    private sealed record BulkCreateUserResult(UserSummaryResponse? CreatedUser, BulkUserFailure? Failure)
    {
        public static BulkCreateUserResult Created(UserSummaryResponse user) => new(user, null);

        public static BulkCreateUserResult Failed(int index, string? email, IReadOnlyList<string> errors)
        {
            return new BulkCreateUserResult(null, new BulkUserFailure(index, email, errors));
        }
    }
}
