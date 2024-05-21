using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class ImageServiceTests
{
    private IImageService? _imageService;
    private IFormFile? _formFile;

    [SetUp]
    public void Setup()
    {
        _imageService = Substitute.For<IImageService>();
        _formFile = Substitute.For<IFormFile>();
    }

    [Test]
    public async Task SaveImage_ShouldReturnString_WhenCalled()
    {
        // Arrange
        var expected = "imagePath";
        _imageService?.SaveImage(_formFile).Returns(expected);

        // Act
        var result = await _imageService?.SaveImage(_formFile);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void SaveImage_ShouldThrowException_WhenImageIsNull()
    {
        // Arrange
        _formFile = null;
        _imageService.When(x => x.SaveImage(_formFile)).Do(x => { throw new ArgumentNullException(); });


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _imageService?.SaveImage(_formFile));
    }
}

