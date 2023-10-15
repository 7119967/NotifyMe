using Microsoft.AspNetCore.Http;

namespace NotifyMe.Infrastructure.Services;

public class UploadFileService
{
    public async Task Upload(string path, string fileName, IFormFile file)
    {
        await using var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
    }
}