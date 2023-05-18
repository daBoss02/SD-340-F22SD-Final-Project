using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public List<SelectListItem> Users { get; set; }

        public ProjectVM(List<ApplicationUser> users)
        {
            Users = new List<SelectListItem>();
            foreach (var user in users)
            {
                Users.Add(new SelectListItem(user.UserName, user.Id));
            }
        }
    }
}
