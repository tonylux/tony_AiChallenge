using Auctions.Models;

namespace Auctions.Data.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly IApplicationDbContext _context;

        public CommentsService(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Add(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment), "Le commentaire ne peut pas être nul");
            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }
    }
}
