

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Threading.Tasks;

namespace Medius
{
    public class AmazonUploader
    {
        public bool sendMyFileToS3(System.IO.Stream localFilePath, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USEast2);
            TransferUtility utility = new TransferUtility(client);
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
            utility.Upload(request); //commensing the transfer  

            return true; //indicate that the file was sent  
        }
        string publicKey = "";
        string secretKey = "";
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
    }
}  
