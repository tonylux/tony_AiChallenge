using Auctions;
using Auctions.Helpers;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class PaginationHelperTests
{
    [Test]
    public void GetPaginatedList_ShouldReturnPaginatedList_WhenCalledWithValidParameters()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var pageNumber = 2;
        var pageSize = 3;
        var expected = new PaginatedList<int>(source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), source.Count, pageNumber, pageSize);

        // Act
        var result = PaginationHelper.GetPaginatedList(source, pageNumber, pageSize);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetPaginatedList_ShouldReturnFirstPage_WhenCalledWithNullPageNumber()
    {
        // Arrange
        var source = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int? pageNumber = null;
        var pageSize = 3;
        var expected = new PaginatedList<int>(source.Take(pageSize).ToList(), source.Count, 1, pageSize);

        // Act
        var result = PaginationHelper.GetPaginatedList(source, pageNumber, pageSize);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
