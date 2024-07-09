namespace SynthShop.Core.Services.Interfaces;

public interface IUserProvider
{
    Guid? GetCurrentUserId();
    string? GetCurrentUserEmail();
    string? GetFullName();
}