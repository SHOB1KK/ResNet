using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
}
