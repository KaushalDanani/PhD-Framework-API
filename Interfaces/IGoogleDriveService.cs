using Backend.Entities;
using Google.Apis.Drive.v3;

namespace Backend.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<DriveService> GetDriveServiceAsync();
        Task SetPublicPermissionsAsync(DriveService service, string fileId);
        Task<FilesResource.CreateMediaUpload> UploadFileToDriveAsync(Stream fileStream, string fileName, string mimeType, string folderId);
        Task<bool> DeleteFileFromDriveAsync(string fileId);
    }
}
