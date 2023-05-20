using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketRepo : IRepository<Ticket>
    {
        public readonly ApplicationDbContext _context;

        public TicketRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> Create(Ticket entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task Delete(Ticket entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Ticket> Get(int id)
        {
            return await _context.Tickets
                .Include(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .Include(t => t.Owner)
                .Include(t => t.TicketWatchers)
                .ThenInclude(tw => tw.Watcher)
                .Include(t => t.Comments)
                .ThenInclude(c => c.CreatedBy)
                .FirstAsync(t => t.Id == id);
        }

        public async Task<ICollection<Ticket>> GetAll()
        {
            return await _context.Tickets
                .Include(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .Include(t => t.Owner)
                .Include(t => t.TicketWatchers)
                .ThenInclude(tw => tw.Watcher)
                .ToListAsync();
        }

        public async Task<Ticket> Update(Ticket entity)
        {
            _context.Tickets.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
