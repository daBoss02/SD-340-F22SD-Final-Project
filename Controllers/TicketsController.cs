using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TicketsBusinessLogic _ticketBLL;

        public TicketsController(IRepository<Project> projectRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IRepository<UserProject> userProjectRepo, IRepository<Ticket> ticketRepo, IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo)
        {
            _ticketBLL = new TicketsBusinessLogic(projectRepo, userManager, contextAccessor, userProjectRepo, ticketRepo, commentRepo, ticketWatcherRepo);
        }
        
        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _ticketBLL.Index());
            } catch
            {
                return BadRequest();
            }
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                return View(await _ticketBLL.Details((int)id));
            } catch
            {
                return NotFound();
            }

        }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create(int projId)
        {
            //Project currProject = _context.Projects.Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).FirstOrDefault(p => p.Id == projId);

            //List<SelectListItem> currUsers = new List<SelectListItem>();
            //currProject.AssignedTo.ToList().ForEach(t =>
            //{
            //    currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            //});

            //ViewBag.Projects = currProject;
            //ViewBag.Users = currUsers;
            try
            {
                return View(await _ticketBLL.Create(projId)) ;
            } catch
            {
                return BadRequest();
            }


        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        {
            if (ModelState.IsValid)
            { 
                await _ticketBLL.CreatePost(ticket, projId, userId);
                return RedirectToAction("Index","Projects", new { area = ""});
            }
            TicketCreateVM vm = await _ticketBLL.Create(projId);
            return View(vm);
        }

		// GET: Tickets/Edit/5
		[Authorize(Roles = "ProjectManager")]
		public async Task<IActionResult> Edit(int? id)
		{
			try
            {
			    return View(await _ticketBLL.Edit((int) id));
            } catch
            {
                return NotFound();
            }

		}

		// GET: Tickets/Edit/5
		[Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                await _ticketBLL.RemoveAssignedUser(id, ticketId);
                return RedirectToAction("Edit", new { id = ticketId });
            } catch
            {
                return NotFound();
            }
            
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id,string userId, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        {
            try
            {
                Ticket currTicket = await _ticketBLL.EditPost(id, userId, ticket);
                return RedirectToAction(nameof(Edit), new {id = currTicket.Id});
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int taskId, string? taskText)
        {
            try
            {
                await _ticketBLL.CommentTask(taskId, taskText);
                return RedirectToAction("Details", new {id = taskId});

            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            try
            {
                await _ticketBLL.UpdateHrs(id, hrs);
                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            }

        public async Task<IActionResult> AddToWatchers(int id)
        {
            try
            {
                await _ticketBLL.AddToWatchers(id);
                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> UnWatch(int id)
        {
            try
            {
                await _ticketBLL.UnWatch(id);
                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            try
            {
                await _ticketBLL.MarkAsCompleted(id);
                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
            try
            {
                await _ticketBLL.UnMarkAsCompleted(id);
                return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }


        // GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                return View(await _ticketBLL.Delete((int) id));
            } catch
            {
                return NotFound();
            }

        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id, int projId)
        {
            try
            {
                await _ticketBLL.DeleteConfirmed(id, projId);
            } catch
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            return RedirectToAction("Index", "Projects");
        }
    }
}

