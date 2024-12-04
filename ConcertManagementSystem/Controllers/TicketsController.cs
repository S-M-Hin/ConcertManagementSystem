
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Website.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using X.PagedList.EF;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace CMS.Website.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ConcertDbContext _context;

        public TicketsController(ConcertDbContext context)
        {
            _context = context;
        }

        // level 0
        [AllowAnonymous] 
        public async Task<IActionResult> Index(string sortOrder, string searchSeatNumberString, string searchBuyerString, int? page)
        {
            int pageCurrent = page ?? 1;  
            int pageMaxSize = 10;

            var tickets = _context.Tickets.Include(m => m.Concert).AsQueryable();
            ViewBag.Concert = new SelectList(_context.Concerts, "Id", "Title");

            // filter
            if (!string.IsNullOrEmpty(searchSeatNumberString))
            {
                tickets = tickets.Where(t => t.SeatNumber.Contains(searchSeatNumberString));
            }

            if (!string.IsNullOrEmpty(searchBuyerString))
            {
                tickets = tickets.Where(t => t.BuyerName.Contains(searchBuyerString));
            }

            // sort
            ViewBag.SortOrder = sortOrder;
            ViewBag.SeatNumberSortParam = string.IsNullOrEmpty(sortOrder) ? "seat-desc" : "";
            ViewBag.PriceSortParam = sortOrder == "price-desc" ? "price-asc" : "price-desc";

            tickets = sortOrder switch
            {
                "seat-desc" => tickets.OrderByDescending(t => t.SeatNumber),
                "price-asc" => tickets.OrderBy(t => t.Price),
                "price-desc" => tickets.OrderByDescending(t => t.Price),
                _ => tickets.OrderBy(t => t.SeatNumber),
            };

            return View(await tickets.ToPagedListAsync(pageCurrent, pageMaxSize));
        }


        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Concert)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // admin only
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ConcertId"] = new SelectList(_context.Set<Concert>(), "Id", "Title");
            return View();
        }

        // admin only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeatNumber,IsSold,Price,PurchaseDate,BuyerName,BuyerEmail,ConcertId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
               _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcertId"] = new SelectList(_context.Set<Concert>(), "Id", "Title");

            
            return View(ticket);
        }

        // GET: Tickets/Edit/5 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["Concerts"] = new SelectList(_context.Concerts, "Id", "Title", ticket.ConcertId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeatNumber,IsSold,Price,PurchaseDate,BuyerName,BuyerEmail,ConcertId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            ViewData["Concerts"] = new SelectList(_context.Concerts, "Id", "Title", ticket.ConcertId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Concert)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5 
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
