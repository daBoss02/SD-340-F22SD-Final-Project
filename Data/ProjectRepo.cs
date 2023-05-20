
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ProjectRepo: IRepository<Project>
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project> Create(Project entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Project> Get(int id)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.AssignedTo)
                .ThenInclude(p => p.ApplicationUser)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Owner)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketWatchers)
                .ThenInclude(tw => tw.Watcher)
                .FirstAsync(p => p.Id == id);
        }


        public async Task<Project> Update(Project entity)
        {
            _context.Projects.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Project entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Project>> GetAll()
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.AssignedTo)
                .ThenInclude(p => p.ApplicationUser)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Owner)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketWatchers)
                .ThenInclude(tw => tw.Watcher)
                .ToListAsync();
        }
    }
}
