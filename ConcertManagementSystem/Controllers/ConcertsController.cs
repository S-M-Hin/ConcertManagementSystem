using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CMS.Website.Data;
using CMS.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.EF;
using Humanizer.Localisation;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Data;
using X.PagedList;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Website.Controllers
{
    
    public class ConcertsController : Controller
    {
        private readonly ConcertDbContext _context;


        public ConcertsController(ConcertDbContext context)
        {
            _context = context;
          
        }

        //GET: Concerts
        public async Task<IActionResult> Index(string sortOrder, int? page, string searchTitleString, string searchPerformerString)
        {
            int pageCurrent = page ?? 1;
            int pageMaxSize = 10;

            var concerts = _context.Concerts.Include(m => m.Venue).AsQueryable();
            ViewBag.Venue = new SelectList(_context.Venues, "Id", "Name");

            if (User.Identity.IsAuthenticated)
            {
               
                ViewBag.SortOrder = sortOrder;
                ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title-desc" : "";
                ViewBag.DateSortParam = sortOrder == "date-desc" ? "date-asc" : "date-desc";

                
                concerts = sortOrder switch
                {
                    "title-desc" => concerts.OrderByDescending(x => x.Title),
                    "date-asc" => concerts.OrderBy(x => x.Date),
                    "date-desc" => concerts.OrderByDescending(x => x.Date),
                    _ => concerts.OrderBy(x => x.Title),
                };

                if (!string.IsNullOrEmpty(searchTitleString))
                {
                    concerts = concerts.Where(m => m.Title.Contains(searchTitleString));
                }

                if (!string.IsNullOrEmpty(searchPerformerString))
                {
                    concerts = concerts.Where(m => m.Performer.Contains(searchPerformerString));
                }
            }
            else
            {
                
                concerts = concerts.OrderBy(x => x.Title);
            }

     
            return View(await concerts.ToPagedListAsync(pageCurrent, pageMaxSize));  
        }


        // GET: Concerts/Details (Read) 
       public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concert = await _context.Concerts
                .Include(c => c.Venue)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concert == null)
            {
                return NotFound();
            }

            return View(concert); 
        }


        // admin - level2 Create
        public IActionResult Create()
        {
         
            ViewData["VenueId"] = new SelectList(_context.Set<Venue>(), "Id", "Name");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Performer,VenueId,Date,TicketPrice,AvailableTickets,IsSoldOut")] Concert concert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Set<Venue>(), "Id", "Name");
           

            return View(concert);

        }


        // admin - level2 Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concert = await _context.Concerts.FindAsync(id);
            if (concert == null)
            {
                return NotFound();
            }

            // Correctly assigning ViewBag.Venues
            ViewData["Venues"]= new SelectList(_context.Venues, "Id", "Name", concert.VenueId);

            return View(concert);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Performer,VenueId,Date,TicketPrice,AvailableTickets,IsSoldOut")] Concert concert)
        {
            if (id != concert.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcertExists(concert.Id))
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
            ViewData["Venues"] = new SelectList(_context.Venues, "Id", "Name", concert.VenueId);
            return View(concert);
        }

        // Admin - level 2 Concert (Delete)

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concert = await _context.Concerts
                .Include(c => c.Venue)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concert == null)
            {
                return NotFound();
            }

            return View(concert); 
        }

      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var concert = await _context.Concerts.FindAsync(id);
            if (concert != null)
            {
                _context.Concerts.Remove(concert);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcertExists(int id)
        {
            return _context.Concerts.Any(e => e.Id == id);
        }
    }
}
