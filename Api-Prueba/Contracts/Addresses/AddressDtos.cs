namespace UserCurrencyApi.Contracts.Addresses;

public sealed record CreateAddressRequest(string Street, string City, string Country, string? ZipCode);

public sealed record UpdateAddressRequest(string Street, string City, string Country, string? ZipCode);

public sealed record AddressResponse(int Id, int UserId, string Street, string City, string Country, string? ZipCode);
