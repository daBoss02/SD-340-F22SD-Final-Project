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
using X.PagedList;
using X.PagedList.Mvc;


namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ProjectBusinessLogic _projectBLL;

        public ProjectsController(IRepository<Project> projectRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IRepository<UserProject> userProjectRepo, IRepository<Ticket> ticketRepo)
        {
            _projectBLL = new ProjectBusinessLogic(projectRepo, userManager, contextAccessor, userProjectRepo, ticketRepo);
        }
        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            try
            {
                ProjectIndexVM vm = await _projectBLL.Index(sortOrder, page, sort, userId);
                return View(vm);
            } catch
            {
                return NotFound();
            }
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                Project project = await _projectBLL.Details((int)id);

                return View(project);
            } catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> RemoveAssignedUser(string id, int projId)
        {
            if (id == null)
            {
                return BadRequest();
            }
            await _projectBLL.RemoveAssignedUser(id, projId);

            return RedirectToAction("Edit", new { id = projId });
        }

        // GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            return View(await _projectBLL.CreateAsync());
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName")] ProjectCreateVM project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                await _projectBLL.Create(project, userIds);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                return View(await _projectBLL.EditGet((int) id));
            } catch
            {
                return BadRequest();
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> userIds, [Bind("Id,ProjectName")] ProjectEditVM project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }
            try
            {
                await _projectBLL.EditPost(id, userIds, project);
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch 
            {
                return NotFound();
            }
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                return View(await _projectBLL.DeleteGet((int)id));
            } catch
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _projectBLL.DeleteConfirmed(id);
                return RedirectToAction("Index");
            } catch
            {
                return NotFound();
            }
        }
    }
}
