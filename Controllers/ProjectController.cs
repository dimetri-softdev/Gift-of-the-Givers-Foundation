using GiftOfTheGivers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Project
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString)
        {
            var projectsQuery = _context.ReliefProjects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmedSearch = searchString.Trim();
                projectsQuery = projectsQuery.Where(p =>
                    EF.Functions.Like(p.Title, $"%{trimmedSearch}%") ||
                    EF.Functions.Like(p.Location, $"%{trimmedSearch}%"));
            }

            var projects = await projectsQuery
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            ViewBag.SearchString = searchString;

            return View(projects);
        }
    }
}