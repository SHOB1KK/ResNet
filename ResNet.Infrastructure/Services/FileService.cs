using Microsoft.AspNetCore.Http;

public class FileService : IFileService
{
    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        var uploadsFolder = Path.Combine("wwwroot", folder);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/{folder}/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine("wwwroot", filePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
