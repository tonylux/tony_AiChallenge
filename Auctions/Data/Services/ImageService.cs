public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveImage(IFormFile image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image), "L'image ne peut pas être nulle");
        }
        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        string filePath = Path.Combine(uploadDir, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return fileName;
    }
}
