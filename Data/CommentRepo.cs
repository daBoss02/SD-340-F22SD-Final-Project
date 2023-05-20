using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class CommentRepo : IRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Comment> Create(Comment entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Comment entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> Get(int id)
        {
            return await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .ThenInclude(c => c.Owner)
                .ThenInclude(o => o.Tickets)
                .Include(c => c.Ticket)
                .ThenInclude(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .FirstAsync( c => c.Id == id);
        }

        public async Task<ICollection<Comment>> GetAll()
        {
            return await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .ThenInclude(c => c.Owner)
                .ThenInclude(o => o.Tickets)
                .Include(c => c.Ticket)
                .ThenInclude(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .ToListAsync();
        }

        public async Task<Comment> Update(Comment entity)
        {
            _context.Comments.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
