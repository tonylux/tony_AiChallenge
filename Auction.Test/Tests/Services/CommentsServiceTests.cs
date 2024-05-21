using NUnit.Framework;
using NSubstitute;
using Auctions.Data.Services;
using Auctions.Models;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Tests;

[TestFixture]
public class CommentsServiceTests
{
    private IApplicationDbContext _context;
    private DbSet<Comment> _dbSet;
    private CommentsService _service;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _dbSet = Substitute.For<DbSet<Comment>, IQueryable<Comment>>();
        _context.Comments.Returns(_dbSet);
        _service = new CommentsService(_context);
    }

    [Test]
    public async Task Add_ShouldCallAddAsyncAndSaveChangesAsync_WhenCommentIsNotNull()
    {
        // Arrange
        var comment = new Comment();

        // Act
        await _service.Add(comment);

        // Assert
        await _dbSet.Received(1).AddAsync(comment);
        await _context.Received(1).SaveChangesAsync();
    }

    [Test]
    public void Add_ShouldThrowArgumentNullException_WhenCommentIsNull()
    {
        // Arrange
        Comment comment = null;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _service.Add(comment));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("comment"));
        Assert.That(ex.Message, Is.EqualTo("Le commentaire ne peut pas être nul (Parameter 'comment')"));
    }
}


