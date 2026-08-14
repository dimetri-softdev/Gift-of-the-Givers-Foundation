using GiftOfTheGivers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Projects
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _context.ReliefProjects.ToListAsync();
            return View(projects);
        }
    }
}