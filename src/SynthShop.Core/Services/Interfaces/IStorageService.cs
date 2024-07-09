namespace SynthShop.Core.Services.Interfaces;

public interface IStorageService
{
    Task UploadAsync(string fileName, Stream stream, string contentType);
}