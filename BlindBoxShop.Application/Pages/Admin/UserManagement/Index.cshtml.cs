using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Application.Pages.Admin.UserManagement
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RepositoryContext _context;

        public IndexModel(UserManager<User> userManager, RepositoryContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<User> Users { get; set; } = new List<User>();
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalReviews { get; set; }

        public async Task OnGetAsync()
        {
            
            Users = await _userManager.Users
                .Include(u => u.Roles)
                .Include(u => u.Orders)
                .Include(u => u.CustomerReviews)
                .ToListAsync();

            
            TotalUsers = Users.Count;
            TotalOrders = Users.Sum(u => u.Orders?.Count ?? 0);
            TotalReviews = Users.Sum(u => u.CustomerReviews?.Count ?? 0);
        }
    }
}

