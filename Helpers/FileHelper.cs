namespace PadhaiHub.Helpers;

public static class FileHelper
{
    private static readonly string[] AllowedImageTypes = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxImageBytes = 2 * 1024 * 1024; // 2 MB

    // Returns an error message, or null if the file is acceptable
    public static string? ValidateImage(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageTypes.Contains(extension))
            return "Only JPG, PNG or WEBP images are allowed.";
        if (file.Length > MaxImageBytes)
            return "The image must be smaller than 2 MB.";
        return null;
    }

    // Saves the file under wwwroot/uploads/<folder> and returns its web path
    public static async Task<string> SaveImageAsync(IFormFile file, string webRootPath, string folder)
    {
        var uploadFolder = Path.Combine(webRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadFolder);

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
        var fullPath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{folder}/{fileName}";
    }

    // Removes an old upload when it is replaced or its record is deleted
    public static void DeleteImage(string? webPath, string webRootPath)
    {
        if (string.IsNullOrEmpty(webPath) || !webPath.StartsWith("/uploads/")) return;

        var fullPath = Path.Combine(webRootPath, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}