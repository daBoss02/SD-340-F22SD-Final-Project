using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;


namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketBusinessLogic
    {
        private readonly ApplicationDbContext _context;

        public TicketBusinessLogic(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task CreateTicketAsync(Ticket newTicket, List<string> userIds, int projId)
        {

            newTicket.ProjectId = projId;
            newTicket.UserIds = userIds;

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateTicketAsync(Ticket updatedTicket, List<string> userIds)
        {
           
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == updatedTicket.Id);
            if (ticket != null)
            {
             
                ticket.UserIds = userIds;

                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        
    }
}
