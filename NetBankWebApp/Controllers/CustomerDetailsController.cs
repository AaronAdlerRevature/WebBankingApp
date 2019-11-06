using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetBankWebApp.Models.Models;
using NetBankWebApp.Models;

namespace NetBankWebApp.Controllers
{
    [Authorize]
    public class CustomerDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.CustomerDetails.ToListAsync());
        }

        // GET: CustomerDetails/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDetailsModel = await _context.CustomerDetails
                .FirstOrDefaultAsync(m => m.Username == id);
            if (customerDetailsModel == null || customerDetailsModel.Username != User.Identity.Name)
            {
                return NotFound();
            }

            return View(customerDetailsModel);
        }

        // GET: CustomerDetails/Create
        public IActionResult Create()
        {
            if (_context.CustomerDetails.Find(User.Identity.Name) != null)
            {
                return RedirectToAction("Index");
            }
            return View(new CustomerDetailsModel());
        }

        // POST: CustomerDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Address,City,State,ZipCode,Country")] CustomerDetailsModel customerDetailsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerDetailsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerDetailsModel);
        }

        // GET: CustomerDetails/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDetailsModel = await _context.CustomerDetails.FindAsync(id);
            if (customerDetailsModel == null || customerDetailsModel.Username != User.Identity.Name)
            {
                return NotFound();
            }
            return View(customerDetailsModel);
        }

        // POST: CustomerDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,Address,City,State,ZipCode,Country")] CustomerDetailsModel customerDetailsModel)
        {
            if (id != customerDetailsModel.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerDetailsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerDetailsModelExists(customerDetailsModel.Username))
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
            return View(customerDetailsModel);
        }

        // GET: CustomerDetails/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerDetailsModel = await _context.CustomerDetails
                .FirstOrDefaultAsync(m => m.Username == id);
            if (customerDetailsModel == null || customerDetailsModel.Username != User.Identity.Name)
            {
                return NotFound();
            }

            return View(customerDetailsModel);
        }

        // POST: CustomerDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customerDetailsModel = await _context.CustomerDetails.FindAsync(id);
            _context.CustomerDetails.Remove(customerDetailsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerDetailsModelExists(string id)
        {
            return _context.CustomerDetails.Any(e => e.Username == id);
        }
    }
}
