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
    public class LoanModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoanModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LoanModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Loan.ToListAsync());
        }

        // GET: LoanModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanModel = await _context.Loan
                .FirstOrDefaultAsync(m => m.id == id);
            if (loanModel == null || loanModel.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            return View(loanModel);
        }

        // GET: LoanModels/Create
        public IActionResult Create()
        {
            var list = _context.Loan.ToList();
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

        // POST: LoanModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,UserName,accId,InterestRate,Balance,toPay")] LoanModel loanModel)
        {
            if (ModelState.IsValid)
            {
                loanModel.accId = "l" + loanModel.accId;
                _context.Add(loanModel);

                var transaction = new TransactionModel();
                transaction.UserName = User.Identity.Name;
                transaction.accId = loanModel.accId;
                transaction.Amount = loanModel.Balance;
                transaction.transferTypeId = 0;
                transaction.CreateTime = DateTime.Now;

                _context.Add(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                var list = _context.Loan.ToList();
                if (list.Count() == 0)
                {
                    ViewData["Count"] = 0;
                }
                else
                {
                    ViewData["Count"] = list[list.Count() - 1].id;
                }
                return View(loanModel);
            }

        }

        public async Task<IActionResult> Pay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanAccount = await _context.Loan.FindAsync(id);
            if (loanAccount == null || loanAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var transferAccount = await _context.Transferable.ToListAsync();
            List<TransferableAccount> toVM = new List<TransferableAccount>();
            foreach (var item in transferAccount)
            {
                if (item.UserName == User.Identity.Name)
                {
                    toVM.Add(item);
                }
            }


            ViewBag.fromAccounts = toVM;
            ViewBag.id = loanAccount.id;
            ViewBag.accId = loanAccount.accId;
            ViewBag.Balance = loanAccount.Balance;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Pay(int id, int fromId, decimal Amount)
        {
           
            var loanAccount = await _context.Loan.FindAsync(id);
            var payAccount = await _context.Transferable.FindAsync(fromId);
            if ((loanAccount == null || loanAccount.UserName != User.Identity.Name) || (payAccount == null || payAccount.UserName != User.Identity.Name) )
            {
                return NotFound();
            }
            if(payAccount.Balance < Amount)
            {
                ModelState.AddModelError("Amount", "You cannot overdraft an account to pay a loan.");
                return View(loanAccount);
            }
           

            var payTransaction = new TransactionModel();
            var paidTransaction = new TransactionModel();
            var closeTransaction = new TransactionModel();

            if (loanAccount.Balance <= Amount)
            {
                Amount = loanAccount.Balance;

                closeTransaction.UserName = User.Identity.Name;
                closeTransaction.accId = loanAccount.accId;
                closeTransaction.Amount = 0;
                closeTransaction.transferTypeId = 5;
                closeTransaction.CreateTime = DateTime.Now;

            }

            loanAccount.Balance -= Amount;

            payTransaction.UserName = User.Identity.Name;
            payTransaction.accId = loanAccount.accId;
            payTransaction.toAccId = payAccount.accId;
            payTransaction.Amount = Amount;
            payTransaction.transferTypeId = 1;
            payTransaction.CreateTime = DateTime.Now;

            payAccount.Balance -= Amount;

            paidTransaction.UserName = User.Identity.Name;
            paidTransaction.accId = payAccount.accId;
            paidTransaction.toAccId = loanAccount.accId;
            paidTransaction.Amount = -Amount;
            paidTransaction.transferTypeId = 2;
            paidTransaction.CreateTime = DateTime.Now;

            try
            {
                if (loanAccount.Balance <= Amount)
                {
                    _context.Loan.Remove(loanAccount);
                    _context.Update(closeTransaction);
                }
                else { _context.Update(loanAccount); }
                _context.Update(payAccount);
                _context.Update(payTransaction);
                _context.Update(paidTransaction);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanModelExists(loanAccount.id))
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


        public async Task<IActionResult> ViewTransactions(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            var transferableModel = await _context.Loan
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

            var transferableModel = await _context.Loan
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


            var loanModel = await _context.Loan
                .FirstOrDefaultAsync(m => m.id == id);
            if (loanModel == null || loanModel.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var AccId = loanModel.accId;

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

            return View("../ViewTransactions/ViewTransactions", TransacVM);
        }

        private bool LoanModelExists(int id)
        {
            return _context.Loan.Any(e => e.id == id);
        }
    }
}
