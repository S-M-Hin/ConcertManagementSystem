using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Website.Data;
using CMS.Data.Entities;
using X.PagedList;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.EF;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS.Website.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ConcertDbContext _context;



        public VenuesController(ConcertDbContext context)
        {
            _context = context;
        }

        //GET:Venues
        public async Task<IActionResult> Index(string sortOrder, int? page, string searchNameString, string searchLocationString)
        {
            int pageCurrent = page ?? 1; // Ако няма зададена страница, започваме от първата.
            int pageMaxSize = 10; // Броят елементи на страница.

            // Започваме с всички места (venues)
            var venues = _context.Venues.AsQueryable();

            // Ако потребителят е влязъл, добавяме параметри за сортиране и търсене
            if (User.Identity.IsAuthenticated)
            {
                // Запазваме параметрите за търсене и сортиране в ViewBag
                ViewBag.SearchName = searchNameString;
                ViewBag.SearchLocation = searchLocationString;
                ViewBag.SortOrder = sortOrder;

                // Филтриране по име на мястото, ако е зададено
                if (!string.IsNullOrEmpty(searchNameString))
                {
                    venues = venues.Where(m => m.Name.Contains(searchNameString));
                }

                // Филтриране по локация на мястото, ако е зададено
                if (!string.IsNullOrEmpty(searchLocationString))
                {
                    venues = venues.Where(m => m.Location.Contains(searchLocationString));
                }

                // Сортиране на резултатите
                venues = sortOrder switch
                {
                    "name-desc" => venues.OrderByDescending(x => x.Name), // Намаляващо по име
                    "capacity-asc" => venues.OrderBy(x => x.Capacity), // Възходящо по капацитет
                    "capacity-desc" => venues.OrderByDescending(x => x.Capacity), // Намаляващо по капацитет
                    _ => venues.OrderBy(x => x.Name), // По подразбиране сортиране по име възходящо
                };
            }
            else
            {
                // Ако потребителят не е влязъл, показваме само местата, сортирани по име
                venues = venues.OrderBy(x => x.Name);
            }

            // Връщаме резултатите с пагинация
            return View(await venues.ToPagedListAsync(pageCurrent, pageMaxSize));
        }


        // GET: Venues/Details/5 (Read) - достъпно за всички потребители
        // Ниво 0 (Гости) и Ниво 1 (Регистрирани потребители) могат да виждат детайлите на мястото
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue); // Показва детайлите за мястото
        }



        // Ниво 2 (Администратори) - Добавяне на ново място (Create)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Capacity,ContactPhone,Email,IsIndoor")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // След създаването пренасочва към списъка с места
            }
            return View(venue); // Ако има грешки, показва отново формуляра за създаване
        }

 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Get the concert from the database
            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return NotFound();
            }

            // Pass the concert model to the view
            return View(venue);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Capacity,ContactPhone,Email,IsIndoor")] Venue venue)
        {
            if (id != venue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // Ниво 2 (Администратори) - Изтриване на място (Delete)
        [Authorize(Roles = "Admin")]
      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);  // Ensure you're passing the correct Venue model
        }


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.Id == id);
        }
    }
}
