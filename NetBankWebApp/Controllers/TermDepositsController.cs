using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetBankWebApp.Models.Models;

namespace NetBankWebApp.Controllers
{
    public class TermDepositsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TermDepositsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TermDeposits
        public async Task<IActionResult> Index()
        {
            return View(await _context.TermDeposit.ToListAsync());
        }

        // GET: TermDeposits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termDeposit = await _context.TermDeposit
                .FirstOrDefaultAsync(m => m.id == id);
            if (termDeposit == null || termDeposit.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            return View(termDeposit);
        }

        // GET: TermDeposits/Create
        public IActionResult Create()
        {
            var list = _context.TermDeposit.ToList();
            if (list.Count() == 0)
            {
                ViewData["Count"] = 0;
            }
            else
            {
                ViewData["Count"] = list[list.Count() - 1].id;
            }
            return View();
        }

        // POST: TermDeposits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,UserName,accId,InterestRate,Balance")] TermDeposit termDeposit)
        {
            if (ModelState.IsValid)
            {
                termDeposit.accId = "t" + termDeposit.accId;
                _context.Add(termDeposit);

                var transaction = new TransactionModel();
                transaction.UserName = User.Identity.Name;
                transaction.accId = termDeposit.accId;
                transaction.Amount = termDeposit.Balance;
                transaction.transferTypeId = 0;
                transaction.CreateTime = DateTime.Now;

                _context.Add(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                var list = _context.TermDeposit.ToList();
                if (list.Count() == 0)
                {
                    ViewData["Count"] = 0;
                }
                else
                {
                    ViewData["Count"] = list[list.Count() - 1].id;
                }
                return View(termDeposit);
            }
        }

        public async Task<IActionResult> ViewTransactions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            var transferableModel = await _context.TermDeposit
                .FirstOrDefaultAsync(m => m.id == id);
            if (transferableModel == null || transferableModel.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var AccId = transferableModel.accId;


            var TransacVM = from t in _context.Transaction
                            where t.accId == AccId
                            orderby t.CreateTime
                            select new TransactionVM
                            {
                                id = t.id,
                                transferTypeId = t.transferTypeId,
                                typeName = null,
                                accId = t.accId,
                                toAccId = t.toAccId,
                                Amount = t.Amount,
                                CreateTime = t.CreateTime
                            };


            ViewBag.VM = TransacVM;

            return View("../ViewTransactions/ViewTransactions");
        }
        [HttpPost]
        public async Task<IActionResult> ViewTransactions(int id, DateTime start, DateTime end)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            var transferableModel = await _context.TermDeposit
                .FirstOrDefaultAsync(m => m.id == id);
            if (transferableModel == null || transferableModel.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var AccId = transferableModel.accId;


            var TransacVM = from t in _context.Transaction
                            where t.accId == AccId && t.CreateTime > start && t.CreateTime < end
                            orderby t.CreateTime
                            select new TransactionVM
                            {
                                id = t.id,
                                transferTypeId = t.transferTypeId,
                                typeName = null,
                                accId = t.accId,
                                toAccId = t.toAccId,
                                Amount = t.Amount,
                                CreateTime = t.CreateTime
                            };


            ViewBag.VM = TransacVM;

            return View("../ViewTransactions/ViewTransactions");
        }

        public async Task<IActionResult> ViewTransactions10(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var CDModel = await _context.TermDeposit
                .FirstOrDefaultAsync(m => m.id == id);
            if (CDModel == null || CDModel.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var AccId = CDModel.accId;

            var TransacVM = (from t in _context.Transaction
                             where t.accId == AccId
                             orderby t.CreateTime
                             select new TransactionVM
                             {
                                 id = t.id,
                                 transferTypeId = t.transferTypeId,
                                 typeName = null,
                                 accId = t.accId,
                                 toAccId = t.toAccId,
                                 Amount = t.Amount,
                                 CreateTime = t.CreateTime
                             }).Take(10);

            ViewBag.VM = TransacVM;
            ViewBag.id = id;

            return View("../ViewTransactions/ViewTransactions");
        }

        private bool TermDepositExists(int id)
        {
            return _context.TermDeposit.Any(e => e.id == id);
        }
    }
}
