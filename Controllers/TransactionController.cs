using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expence_Tracker.Models;

namespace Expence_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Transactions.Include(t => t.Category);
            return View(await appDbContext.ToListAsync());
        }


        // GET: Transaction/Create
        public IActionResult CreateOrEdit(int id=0)
        {
            populateCategries();
            if(id==0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit([Bind("Id,CategoryID,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.Id == 0)
                    _context.Add(transaction);
                else
                    _context.Update(transaction);
				await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            populateCategries();
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "Id", "Id", transaction.CategoryID);
            return View(transaction);
        }
        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'AppDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void populateCategries()
        {
            var CategoryCollection =  _context.Categories.ToList();
            Category category = new Category() {Id = 0, Title="Chose A Category" };
            CategoryCollection.Insert(0,category);
            ViewBag.Category = CategoryCollection;
        }
    }
}
