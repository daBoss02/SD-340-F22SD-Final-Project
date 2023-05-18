﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Web.Mvc;
using X.PagedList;
using Project = SD_340_W22SD_Final_Project_Group6.Models.Project;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectBusinessLogic
    {
        private readonly IRepository<Project> _projectRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRepository<UserProject> _userProjectRepo;
        private readonly IRepository<Ticket> _ticketRepo;

        public ProjectBusinessLogic(IRepository<Project> projectRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IRepository<UserProject> userProjectRepo, IRepository<Ticket> ticketRepo)
        {
            _projectRepo = projectRepo;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _userProjectRepo = userProjectRepo;
            _ticketRepo = ticketRepo;
        }


        public async Task<ProjectIndexVM> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            ProjectIndexVM vm = new ProjectIndexVM();
            List<Project> SortedProjs = new List<Project>();
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            vm.Users = users;
            SortedProjs = _projectRepo.GetAll().ToList();
            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderByDescending(t => t.TicketPriority)
                                .ToList();
                        }
                    }
                    else
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderBy(t => t.TicketPriority)
                                .ToList();
                        }
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderByDescending(t => t.RequiredHours)
                                .ToList();
                        }
                    }
                    else
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderBy(t => t.RequiredHours)
                                .ToList();
                        }
                    }

                    break;
                case "Completed":
                    foreach (Project project in SortedProjs)
                    {
                        project.Tickets = project.Tickets
                            .Where(t => t.Completed == true)
                            .ToList();
                    }
                    break;
                default:
                    if (userId != null)
                    {
                        SortedProjs = SortedProjs
                            .OrderBy(p => p.ProjectName)
                            .ToList();
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .Where(t => t.Owner.Id.Equals(userId)).ToList();
                        }
                    }
                    else
                    {
                        SortedProjs = SortedProjs.OrderBy(p => p.ProjectName).ToList();
                    }

                    break;
            }
            //check if User is PM or Develoer
            var LoggedUserName = _contextAccessor.HttpContext.User.Identity.Name;  // logined user name
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == LoggedUserName);
            var rolenames = await _userManager.GetRolesAsync(user);
            var AssignedProject = new List<Project>();
            // geting assined project
            if (rolenames.Contains("Developer"))
            {
                AssignedProject = SortedProjs.Where(p => p.AssignedTo.Select(projectUser => projectUser.UserId).Contains(user.Id)).ToList();
            }
            else
            {
                AssignedProject = SortedProjs;
            }
            X.PagedList.IPagedList<Project> projList = AssignedProject.ToPagedList(page ?? 1, 3);

            vm.Projects = projList;
            return vm;
        }

        public Project Details(int id)
        {
            Project project = _projectRepo.Get(id);
            return project;
        }

        public void RemoveAssignedUser(string id, int projId)
        {
            UserProject userProject = _userProjectRepo.GetAll()
                .First(up => up.UserId == id && up.ProjectId == projId);

            _userProjectRepo.Delete(userProject);
            return;
        }

        public async Task<ProjectCreateVM> CreateAsync()
        {
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });

            ProjectCreateVM vm = new ProjectCreateVM();

            vm.Users = users;

            return vm;
        }

        public async void Create(ProjectCreateVM vm, List<string> userIds)
        {
            string userName = _contextAccessor.HttpContext.User.Identity.Name;

            Project project = new Project();

            project.ProjectName = vm.ProjectName;

            ApplicationUser createdBy = _userManager.Users.First(u => u.UserName == userName);
            project.CreatedBy = createdBy;
            userIds.ForEach((user) =>
            {
                ApplicationUser currUser = _userManager.Users.FirstOrDefault(u => u.Id == user);
                UserProject newUserProj = new UserProject();
                newUserProj.ApplicationUser = currUser;
                newUserProj.UserId = currUser.Id;
                newUserProj.Project = project;
                project.AssignedTo.Add(newUserProj);
                _userProjectRepo.Create(newUserProj);
            });
            return;
        }

        public async Task<ProjectEditVM> EditGet(int id)
        {
            var project = _projectRepo.Get(id);

            List<ApplicationUser> results = _userManager.Users.ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ProjectEditVM vm = new ProjectEditVM();
            vm.Project = project;
            vm.CurrUsers = currUsers;

            return vm;
        }

        public async void EditPost(int id, List<string> userIds, ProjectEditVM projectEditVM)
        {
            userIds.ForEach((user) =>
            {
                ApplicationUser currUser = _userManager.Users.FirstOrDefault(u => u.Id == user);
                UserProject newUserProj = new UserProject();
                newUserProj.ApplicationUser = currUser;
                newUserProj.UserId = currUser.Id;
                newUserProj.Project = projectEditVM.Project;
                projectEditVM.Project.AssignedTo.Add(newUserProj);
            });
        }

        public async Task<Project> DeleteGet(int id)
        {
            Project project = _projectRepo.Get(id);
            return project;
        }

        public async void DeleteConfirmed(int id)
        {
            Project project = _projectRepo.Get(id);
            if (project != null)
            {
                List<Ticket> tickets = project.Tickets.ToList();
                tickets.ForEach(ticket =>
                {
                    _ticketRepo.Delete(ticket);
                });
                List<UserProject> userProjects = _userProjectRepo.GetAll().Where(up => up.ProjectId == project.Id).ToList();
                userProjects.ForEach(userProj =>
                {
                    _userProjectRepo.Delete(userProj);
                });

                _projectRepo.Delete(project);
            }
        }

        private async Task<bool> ProjectExists(int id)
        {
            List<Project> projects = await _projectRepo.GetAll().ToListAsync();
            if (projects.Any(p => p.Id == id))
            {
                return true;
            }
            return false;
        }
    }
}
