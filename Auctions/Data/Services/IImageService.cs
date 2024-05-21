public interface IImageService
{
    Task<string> SaveImage(IFormFile image);
}
