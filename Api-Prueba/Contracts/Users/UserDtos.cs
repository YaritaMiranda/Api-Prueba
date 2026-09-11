using UserCurrencyApi.Contracts.Addresses;

namespace UserCurrencyApi.Contracts.Users;

public sealed record CreateUserRequest(string Name, string Email);

public sealed record UpdateUserRequest(string Name, string Email, bool IsActive);

public sealed record BulkCreateUsersRequest(IReadOnlyList<CreateUserRequest> Users);

public sealed record UserSummaryResponse(int Id, string Name, string Email, bool IsActive);

public sealed record UserDetailResponse(
    int Id,
    string Name,
    string Email,
    bool IsActive,
    IReadOnlyList<AddressResponse> Addresses);

public sealed record BulkUserFailure(int Index, string? Email, IReadOnlyList<string> Errors);

public sealed record BulkCreateUsersResponse(
    int CreatedCount,
    IReadOnlyList<UserSummaryResponse> CreatedUsers,
    IReadOnlyList<BulkUserFailure> FailedUsers);
