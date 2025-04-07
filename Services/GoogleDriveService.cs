using Backend.Entities;
using System.IO;
using Backend.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Drive.v3.Data;

namespace Backend.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private static string[] Scopes = { DriveService.Scope.Drive };

        public GoogleDriveService() { }

        // Get Google API Service Client
        public async Task<DriveService> GetDriveServiceAsync()
        {
            //UserCredential credential;

            //await using (var stream = new FileStream(@"D:\Computer Center\phd_framework_client_secret.json", FileMode.Open, FileAccess.Read))
            //{
            //    const string folderPath = @"D:\";
            //    string filePath = Path.Combine(folderPath, "GoogleDriveServiceCredentials.json");

            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync
            //    (
            //        GoogleClientSecrets.FromStream(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(filePath, true)
            //    ).Result;
            //}

            GoogleCredential credential;

            await using (var stream = new FileStream(@"D:\Computer Center\phd-framework-service-account-secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "PhD-Framework"
            });
        }

        public async Task SetPublicPermissionsAsync(DriveService driveService, string fileId)
        {
            var permission = new Permission()
            {
                Type = "anyone", // Make the file accessible to anyone
                Role = "reader"  // Grant read access
            };

            try
            {
                // Create the permission
                await driveService.Permissions.Create(permission, fileId).ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting permissions: {ex.Message}");
            }
        }

        // Upload File to Google Drive
        public async Task<FilesResource.CreateMediaUpload> UploadFileToDriveAsync(Stream fileStream, string fileName, string mimeType, string folderId)
        {
            var driveService = await GetDriveServiceAsync();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { folderId },
                MimeType = mimeType
            };

            var request = driveService.Files.Create(fileMetadata, fileStream, mimeType);
            request.Fields = "id, name, mimeType, size";
            try
            {
                var file = await request.UploadAsync();

                if (file.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    throw new Exception("Error uploading file: " + file.Exception?.Message);
                }

                if (string.IsNullOrEmpty(request.ResponseBody.Id))
                {
                    throw new Exception("File ID is not available in the response.");
                }

                // Set file permissions to make it public
                await SetPublicPermissionsAsync(driveService, request.ResponseBody.Id);

                // Return the URL of the file
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading file: " + ex.Message);
            }
        }

        // Delete uploaded file using FileId which is StoredCloudFileId
        public async Task<bool> DeleteFileFromDriveAsync(string fileId)
        {
            var driveService = await GetDriveServiceAsync();

            try
            {
                var request = driveService.Files.Delete(fileId);
                await request.ExecuteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
