using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketWatcherRepo : IRepository<TicketWatcher>
    {
        private readonly ApplicationDbContext _context;

        public TicketWatcherRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TicketWatcher> Create(TicketWatcher entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(TicketWatcher entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TicketWatcher> Get(int id)
        {
            return await _context.TicketWatchers
                .Include(tw => tw.Ticket)
                .Include(tw => tw.Watcher)
                .FirstAsync(tw => tw.Id == id);
        }

        public async Task<ICollection<TicketWatcher>> GetAll()
        {
            return await _context.TicketWatchers
                .Include(tw => tw.Ticket)
                .Include(tw => tw.Watcher)
                .ToListAsync();
        }

        public async Task<TicketWatcher> Update(TicketWatcher entity)
        {
            _context.TicketWatchers.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
