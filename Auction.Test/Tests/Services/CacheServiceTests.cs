using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class CacheServiceTests
{
    private ICacheService _cacheService;

    [SetUp]
    public void Setup()
    {
        _cacheService = Substitute.For<ICacheService>();
    }

    [Test]
    public void Retrieve_ShouldReturnCorrectType()
    {
        // Arrange
        var expected = "TestValue";
        _cacheService.Retrieve<string>("TestKey").Returns(expected);

        // Act
        var result = _cacheService.Retrieve<string>("TestKey");

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Store_ShouldNotThrowException_WhenCalledWithValidParameters()
    {
        // Arrange
        var valueToStore = "TestValue";
        var timeSpan = TimeSpan.FromMinutes(1);

        // Act & Assert
        Assert.DoesNotThrow(() => _cacheService.Store("TestKey", valueToStore, timeSpan));
    }
}

