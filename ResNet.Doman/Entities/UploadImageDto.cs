using Microsoft.AspNetCore.Http;

public class UploadImageDto
{
    public IFormFile Image { get; set; } = null!;
}
