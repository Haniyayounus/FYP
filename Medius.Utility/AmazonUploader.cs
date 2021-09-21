

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Medius
{
    public class AmazonUploader
    {
        string publicKey = "";
        string secretKey = "";
        public bool sendMyFileToS3(System.IO.Stream localFilePath, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            var credentials = new BasicAWSCredentials(publicKey, secretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };
            using var client = new AmazonS3Client(credentials, config);
            var fileTransferUtility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name  
            }
            else
            {   // subdirectory and bucket name  
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }
            request.Key = fileNameInS3; //file name up in S3  
            request.InputStream = localFilePath;
            fileTransferUtility.Upload(request); //commensing the transfer  

            return true; //indicate that the file was sent  
        }
       
        public async Task<bool> DeleteFile(string filename, string bucketName)
        {
            string key = filename;
            var credentials = new BasicAWSCredentials(publicKey, secretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };
            using var client = new AmazonS3Client(credentials, config);
            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = key
            });
            return true;
        }
        public class ServiceConfiguration
        {
            public AWSS3Configuration AWSS3 { get; set; }
        }
        public class AWSS3Configuration
        {
            public string BucketName { get; set; }
        }
        public interface IAWSS3BucketHelper
        {
            Task<bool> UploadFile(System.IO.Stream inputStream, string fileName);
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
}
