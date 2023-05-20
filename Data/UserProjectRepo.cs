
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserProjectRepo: IRepository<UserProject>
    {
        private readonly ApplicationDbContext _context;

        public UserProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProject> Create(UserProject entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<UserProject> Get(int id)
        {
            return await _context.UserProjects
                .Include(up => up.Project)
                .Include(up => up.ApplicationUser)
                .FirstAsync(up => up.Id == id);
        }


        public async Task<UserProject> Update(UserProject entity)
        {
            _context.UserProjects.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(UserProject entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<UserProject>> GetAll()
        {
            return await _context.UserProjects
                .Include(up => up.Project)
                .Include(up => up.ApplicationUser)
                .ToListAsync();
        }
    }
}
