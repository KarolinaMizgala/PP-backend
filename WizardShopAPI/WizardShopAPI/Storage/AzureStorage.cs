﻿using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using WizardShopAPI.DTOs;
using WizardShopAPI.ResponseDto;
using WizardShopAPI.Services;
using System.Reflection.Metadata;
using WizardShopAPI.Models;

namespace WizardShopAPI.Storage
{
    public class AzureStorage : IAzureStorage
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureStorage> _logger;

        public AzureStorage(IConfiguration config, ILogger<AzureStorage> logger)
        {
            _storageConnectionString = config.GetValue<string>("BlobConnectionString");
            _storageContainerName = config.GetValue<string>("BlobContainerName");
            _logger = logger;
        }

        public async Task<ImageResponseDto> UploadAsync(IFormFile file, int productId)
        {
            string imageId = productId.ToString() + "_";
            int id = await this.GetNewProductImageId(productId);
            imageId += id.ToString();

            // Create new upload response object that we can return to the requesting method
            ImageResponseDto response = new();
            // Get a reference to a container named in appsettings.json and then create it
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            //await container.CreateAsync();
            try
            {
                if (file != null)
                {
                    Guid guid = Guid.NewGuid();
                    string extension = Path.GetExtension(file.FileName);

                    string uploadpath = Path.Combine(Path.GetDirectoryName(file.FileName), imageId + extension);
                    var stream = new FileStream(uploadpath, FileMode.Create);

                    file.CopyTo(stream);
                    FormFile blobRenamed = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                    file = blobRenamed;
                }
                // Get a reference to the blob just uploaded from the API in a container from configuration settings
                BlobClient client = container.GetBlobClient(file.FileName);

                // Open a stream for the file we want to upload
                await using (Stream? data = file.OpenReadStream())
                {
                    // Upload the file async
                    await client.UploadAsync(data);
                }

                // Everything is OK and file got uploaded
                response.Status = $"File {file.FileName} Uploaded Successfully";
                response.Error = false;
                response.Image.Uri = client.Uri.AbsoluteUri;
                response.Image.Name = client.Name;
            }
            // If the file already exists, we catch the exception and do not upload it
            catch (RequestFailedException ex)
               when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                _logger.LogError($"File with name {file.FileName} already exists in container. Set another name to store the file in the container: '{_storageContainerName}.'");
                response.Status = $"File with name {file.FileName} already exists. Please use another name to store your file.";
                response.Error = true;
                return response;
            }
            // If we get an unexpected error, we catch it here and return the error message
            catch (RequestFailedException ex)
            {
                // Log error to console and create a new response we can return to the requesting method
                _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
                response.Error = true;
                return response;
            }

            // Return the BlobUploadResponse object
            return response;
        }

        //returns only images that ar used in products (not in reviews)
        public async Task<List<ImageDto>> ListAsync()
        {
            // Get a reference to a container named in appsettings.json
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            // Create a new list object for 
            List<ImageDto> files = new List<ImageDto>();

            await foreach (BlobItem file in container.GetBlobsAsync())
            {
                // Add each file retrieved from the storage container to the files list by creating a BlobDto object
                string uri = container.Uri.ToString();
                var name = file.Name;

                if (!name.Contains('R'))
                {
                    var fullUri = $"{uri}/{name}";

                    files.Add(new ImageDto
                    {
                        Uri = fullUri,
                        Name = name,
                        ContentType = file.Properties.ContentType
                    });
                }
            }

            // Return all files to the requesting method
            return files;
        }

        public async Task<bool> DeleteAllImagesFromEntityAsync(int productId)
        {
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            List<ImageDto> matchingImagesToPrID = await this.GetListOfImagesForProduct(productId);

            foreach (ImageDto image in matchingImagesToPrID)
            {
                BlobClient clientFile = client.GetBlobClient(image.Name);
                try
                {
                    // Delete the file
                    await clientFile.DeleteAsync();
                }
                catch (RequestFailedException ex)
                    when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
                {
                    return false;
                }
            }
            // Return a new BlobResponseDto to the requesting method
            return true;
        }

        public async Task<List<string>> GetListOfAllUrisForEntityAsync(int productId)
        {
            string firstPart = productId.ToString() + "_";

            List<string> matchingImages = new List<string>();
            // Get a reference to a container named in appsettings.json
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            await foreach (BlobItem file in container.GetBlobsAsync())
            {
                var name = file.Name;
                string uri = container.Uri.ToString();

                if (name.Length > firstPart.Length)
                {
                    string substring = name.Substring(0, firstPart.Length);
                    if (substring == firstPart)
                    {
                        var fullUri = $"{uri}/{name}";
                        matchingImages.Add(fullUri);
                    }
                }
            }

            return matchingImages;
        }

        private async Task<List<ImageDto>> GetListOfImagesForProduct(int productId)
        {
            string firstPart = productId.ToString() + "_";

            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            List<ImageDto> matchingImages = new List<ImageDto>();

            await foreach (BlobItem file in container.GetBlobsAsync())
            {
                var name = file.Name;
                string uri = container.Uri.ToString();

                if (name.Length > firstPart.Length)
                {
                    string substring = name.Substring(0, firstPart.Length);
                    if (substring == firstPart)
                    {
                        var fullUri = $"{uri}/{name}";
                        matchingImages.Add(new ImageDto
                        {
                            Uri = fullUri,
                            Name = name,
                            ContentType = file.Properties.ContentType
                        });
                    }
                }
            }

            return matchingImages;
        }
        private async Task<int> GetNewProductImageId(int productId)
        {
            string firstPart = productId.ToString() + "_";
            // Get a reference to a container named in appsettings.json
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            SortedSet<int> productImageIds = new SortedSet<int>();

            await foreach (BlobItem file in container.GetBlobsAsync())
            {
                var name = file.Name;

                if (name.Length > firstPart.Length)
                {
                    string substring = name.Substring(0, firstPart.Length);
                    if (substring == firstPart)
                    {
                        string idWithExtension = name.Substring(firstPart.Length); //from 2_123.jpg returs 123.jpg
                        productImageIds.Add(GetImageId(idWithExtension));
                    }
                }
            }

            int maxId = productImageIds.LastOrDefault() + 1;
            return maxId;
        }

        //from ex. 123.jpg returns 123
        private int GetImageId(string idWithExtension)
        {
            string id = String.Empty;
            foreach (char c in idWithExtension)
            {
                if (c == '.')
                {
                    break;
                }
                id += c;
            }
            return Int32.Parse(id);
        }
    }
}
