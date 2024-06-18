using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Settings;

namespace SynthShop.Core.Services.Impl
{
    public class StorageService: IStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AWSSettings _settings;

        public StorageService(IAmazonS3 s3client, IOptions<AWSSettings> settings)
        {
            _s3Client = s3client;
            _settings = settings.Value;
        }

        public async Task UploadAsync(string fileName, Stream stream, string contentType)
        {
            var putRequest = new PutObjectRequest()
            {
                BucketName = _settings.BucketName,
                Key = fileName,
                ContentType = contentType,
                InputStream = stream
            };
            await _s3Client.PutObjectAsync(putRequest);
        }
        
    }
}
