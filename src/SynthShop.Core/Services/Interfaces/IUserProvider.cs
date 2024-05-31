namespace SynthShop.Core.Services.Interfaces
{
    public interface IUserProvider
    {
        public Guid? GetCurrentUserId();
    }
}
