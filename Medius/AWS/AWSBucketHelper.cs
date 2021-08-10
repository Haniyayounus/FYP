using Amazon.S3;
using Amazon.S3.Model;
using Medius.Model.AWS;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Medius.AWS
{
    public interface IAWSS3BucketHelper
    {
        Task<bool> UploadFile(System.IO.Stream inputStream, string fileName);
        Task<ListVersionsResponse> FilesList();
        Task<Stream> GetFile(string key);
        Task<bool> DeleteFile(string key);
    }
    public class AWSS3BucketHelper : IAWSS3BucketHelper
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ServiceConfiguration _settings;
        public AWSS3BucketHelper(IAmazonS3 s3Client, IOptions<ServiceConfiguration> settings)
        {
            this._amazonS3 = s3Client;
            this._settings = settings.Value;
        }
        public async Task<bool> UploadFile(System.IO.Stream inputStream, string fileName)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = inputStream,
                    BucketName = _settings.AWSS3.BucketName,
                    Key = fileName
                };
                PutObjectResponse response = await _amazonS3.PutObjectAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<ListVersionsResponse> FilesList()
        {
            return await _amazonS3.ListVersionsAsync(_settings.AWSS3.BucketName);
        }
        public async Task<Stream> GetFile(string key)
        {

            GetObjectResponse response = await _amazonS3.GetObjectAsync(_settings.AWSS3.BucketName, key);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return response.ResponseStream;
            else
                return null;
        }

        public async Task<bool> DeleteFile(string key)
        {
            try
            {
                DeleteObjectResponse response = await _amazonS3.DeleteObjectAsync(_settings.AWSS3.BucketName, key);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
