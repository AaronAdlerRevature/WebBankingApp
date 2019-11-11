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
using Microsoft.AspNetCore.Identity;

namespace NetBankWebApp.Controllers
{   [Authorize]//, Route("/Customer/{action=Index}/{Username?}")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer.ToListAsync());
        }

        // GET: Customer/Details/5
        //[Route("/Customer/Details/{Username?}")]
        public async Task<IActionResult> Details(string Username)
        {
            if (Username == null)
            {
                return NotFound();
            }

            var customerModel = await _context.Customer
                .FirstOrDefaultAsync(m => m.Username == Username);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {

            if(_context.Customer.Find(User.Identity.Name) != null)
            {
                return RedirectToAction("Index");
            }
            return View(new CustomerModel());
        }

        // POST: Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Username,FirstName,LastName,DOB,SSN")] CustomerModel customerModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerModel);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(string Username)
        {
            if (Username == null)
            {
                return NotFound();
            }

            var customerModel = await _context.Customer.FindAsync(Username);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Username, [Bind("Username,FirstName,LastName,DOB,SSN")] CustomerModel customerModel)
        {
            if (Username != customerModel.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerModelExists(customerModel.Username))
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
            return View(customerModel);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(string Username)
        {
            if (Username == null)
            {
                return NotFound();
            }

            var customerModel = await _context.Customer
                .FirstOrDefaultAsync(m => m.Username == Username);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Username)
        {
            var customerModel = await _context.Customer.FindAsync(Username);
            _context.Customer.Remove(customerModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerModelExists(string Username)
        {
            return _context.Customer.Any(e => e.Username == Username);
        }

        public async Task<IActionResult> ViewAllTrans()
        {
            ViewBag.trans = _context.Transaction.Where(x => x.UserName == User.Identity.Name);

            return View();
        }

    }
}
