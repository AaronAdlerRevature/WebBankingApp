using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetBankWebApp.Models.Models;
using Microsoft.AspNetCore.Authorization;

namespace NetBankWebApp.Controllers
{
    [Authorize]
    public class TransferableAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransferableAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TransferableAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transferable.ToListAsync());
        }

        // GET: TransferableAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var TransferableAccount = await _context.Transferable
                .FirstOrDefaultAsync(m => m.id == id);
            if (TransferableAccount == null || TransferableAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            return View(TransferableAccount);
        }

        // GET: TransferableAccounts/Create
        public IActionResult Create()
        {
            var list = _context.Transferable.ToList();
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

        // POST: TransferableAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id, accId, UserName, InterestRate, Balance, CanOverdraft")] TransferableAccount TransferableAccount)
        {
            if (TransferableAccount.CanOverdraft == false)
            {
                TransferableAccount.accId = 'c' + TransferableAccount.accId;
                TransferableAccount.InterestRate = 0.0008M;
            }
            else
            {
                TransferableAccount.accId = 'b' + TransferableAccount.accId;
                TransferableAccount.InterestRate = 0.001M;
            }
            if (ModelState.IsValid)
            {
                

                var transaction = new TransactionModel();
                transaction.UserName = User.Identity.Name;
                transaction.accId = TransferableAccount.accId;
                transaction.Amount = 0;
                transaction.transferTypeId = 0;
                transaction.CreateTime = DateTime.Now;

                _context.Add(transaction);

                _context.Add(TransferableAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(TransferableAccount);
        }
 

        //Withdraw Functions
        public async Task<IActionResult> Withdraw(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var TransferableAccount = await _context.Transferable.FindAsync(id);
            TransferableAccount.ToTransfer = null;
            if (TransferableAccount == null || TransferableAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            return View(TransferableAccount);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id, [Bind("id,accId,UserName,Balance, InterestRate, ToTransfer")] TransferableAccount TransferableAccount)
        {

            if (id != TransferableAccount.id)
            {
                return NotFound();
            }

            if (TransferableAccount.ToTransfer is null)
            {
                ModelState.AddModelError("ToTransfer", "Please enter a positive value.");
                return View(TransferableAccount);
            }

            var transaction = new TransactionModel();
            var transactionfee = new TransactionModel();


            if (ModelState.IsValid)
            {

                if (TransferableAccount.accId[0] == 'c')
                {
                    if ((TransferableAccount.Balance - TransferableAccount.ToTransfer) < 0)
                    {
                        return View("../ErrorMessages/CannotOverdraft");

                    }
                    else
                    {
                        transaction.UserName = User.Identity.Name;
                        transaction.accId = TransferableAccount.accId;
                        transaction.Amount = -(decimal)TransferableAccount.ToTransfer;
                        transaction.transferTypeId = 2;
                        transaction.CreateTime = DateTime.Now;

                        TransferableAccount.Balance -= (decimal)TransferableAccount.ToTransfer;
                        TransferableAccount.ToTransfer = null;
                    }
                }
                else if (TransferableAccount.accId[0] == 'b')
                {
                    float fee = 0;             
                    if ((TransferableAccount.Balance - TransferableAccount.ToTransfer) < 0)
                    {
                        if (TransferableAccount.Balance < 0)
                        {
                            var CurrBal = (float)TransferableAccount.Balance;
                            var val = (float)(TransferableAccount.Balance - TransferableAccount.ToTransfer);
                            fee -= (float)((val-CurrBal) * 0.1);
                        }
                        else
                        {
                            var val = (float)(TransferableAccount.Balance - TransferableAccount.ToTransfer);
                            fee -= (float)(val * 0.1);
                        }
                    }
                    transactionfee.UserName = User.Identity.Name;
                    transactionfee.accId = TransferableAccount.accId;
                    transactionfee.Amount = -(decimal)fee;
                    transactionfee.transferTypeId = 4;
                    transactionfee.CreateTime = DateTime.Now;

                    transaction.UserName = User.Identity.Name;
                    transaction.accId = TransferableAccount.accId;
                    transaction.Amount = -(decimal)TransferableAccount.ToTransfer;
                    transaction.transferTypeId = 2;
                    transaction.CreateTime = DateTime.Now;

                    TransferableAccount.Balance -= (decimal)TransferableAccount.ToTransfer + (decimal)fee;
                    TransferableAccount.ToTransfer = null;
                }

                

                try
                {
                    if(transactionfee.Amount != 0)
                    {
                        _context.Update(transactionfee);
                    }
                    _context.Update(TransferableAccount);
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferableAccountExists(TransferableAccount.id))
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
            return View(TransferableAccount);
        }

        public async Task<IActionResult> Deposit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var TransferableAccount = await _context.Transferable.FindAsync(id);
            TransferableAccount.ToTransfer = null;
            if (TransferableAccount == null || TransferableAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            return View(TransferableAccount);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int id, [Bind("id, accId, UserName, Balance, InterestRate, ToTransfer")] TransferableAccount transferableAccount)
        {
            var transaction = new TransactionModel();
            if (id != transferableAccount.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                transaction.UserName = User.Identity.Name;
                transaction.accId = transferableAccount.accId;
                transaction.Amount = (decimal)transferableAccount.ToTransfer;
                transaction.transferTypeId = 1;
                transaction.CreateTime = DateTime.Now;

                transferableAccount.Balance += (decimal)transferableAccount.ToTransfer;
                transferableAccount.ToTransfer = null;

                try
                {
                    _context.Update(transaction);
                    _context.Update(transferableAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferableAccountExists(transferableAccount.id))
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
            return View(transferableAccount);
        }

        //Transfer
        public async Task<IActionResult> Transfer(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var transferableAccount = await _context.Transferable.FindAsync(id);
            transferableAccount.ToTransfer = null;
            if (transferableAccount == null || transferableAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var TransferableList = await _context.Transferable.ToListAsync();
            List<TransferableAccount> remove = new List<TransferableAccount>();
            remove.Add(transferableAccount);
            foreach (var item in TransferableList)
            {
                if (item.UserName != User.Identity.Name)
                {
                    remove.Add(item);
                }
            }
            foreach (var item in remove)
            {
                TransferableList.Remove(item);
            }

            ViewData["toAcc"] = new SelectList(TransferableList, "id", "accId");

            var transfer = new Transfer();
            transfer.fromId = id;
            transfer.fromAccId = transferableAccount.accId;

            return View(transfer) ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(Transfer transfer)
        {

            if (transfer.fromId == transfer.toId)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {

            var TransferableAccount = await _context.Transferable.FindAsync(transfer.fromId);
            var toTransferAcc = await _context.Transferable.FindAsync(transfer.toId);

            var transactionFrom = new TransactionModel();
            var transactionTo = new TransactionModel();
            var transactionfee = new TransactionModel();

                if (TransferableAccount.accId[0] == 'c')
                {
                    if ((TransferableAccount.Balance - transfer.Amount) < 0)
                    {
                        return View("../ErrorMessages/CannotOverdraft");

                    }
                    else
                    {
                        transactionFrom.UserName = User.Identity.Name;
                        transactionFrom.accId = TransferableAccount.accId;
                        transactionFrom.toAccId = toTransferAcc.accId;
                        transactionFrom.Amount = -transfer.Amount;
                        transactionFrom.transferTypeId = 3;
                        transactionFrom.CreateTime = DateTime.Now;

                        TransferableAccount.Balance -= transfer.Amount;

                        transactionTo.UserName = User.Identity.Name;
                        transactionTo.accId = toTransferAcc.accId;
                        transactionTo.toAccId = transfer.fromAccId;
                        transactionTo.Amount = transfer.Amount;
                        transactionTo.transferTypeId = 3;
                        transactionTo.CreateTime = DateTime.Now;

                        toTransferAcc.Balance += transfer.Amount;

                    }
                }
                else if (TransferableAccount.accId[0] == 'b')
                {
                    float fee = 0;
                    if ((TransferableAccount.Balance - transfer.Amount) < 0)
                    {
                        if (TransferableAccount.Balance < 0)
                        {
                            var CurrBal = (float)TransferableAccount.Balance;
                            var val = (float)(TransferableAccount.Balance - transfer.Amount);
                            fee -= (float)((val - CurrBal) * 0.1);
                        }
                        else
                        {
                            var val = (float)(TransferableAccount.Balance - transfer.Amount);
                            fee -= (float)(val * 0.1);
                        }
                    }
                    transactionfee.UserName = User.Identity.Name;
                    transactionfee.accId = TransferableAccount.accId;
                    transactionfee.Amount = -(decimal)fee;
                    transactionfee.transferTypeId = 4;
                    transactionfee.CreateTime = DateTime.Now;

                    transactionFrom.UserName = User.Identity.Name;
                    transactionFrom.accId = TransferableAccount.accId;
                    transactionFrom.toAccId = toTransferAcc.accId;
                    transactionFrom.Amount = -transfer.Amount;
                    transactionFrom.transferTypeId = 3;
                    transactionFrom.CreateTime = DateTime.Now;

                    TransferableAccount.Balance -= transfer.Amount + (decimal)fee;

                    transactionTo.UserName = User.Identity.Name;
                    transactionTo.accId = toTransferAcc.accId;
                    transactionTo.toAccId = transfer.fromAccId;
                    transactionTo.Amount = transfer.Amount;
                    transactionTo.transferTypeId = 3;
                    transactionTo.CreateTime = DateTime.Now;

                    toTransferAcc.Balance += transfer.Amount;

                }

                try
                {
                    if (transactionfee.Amount != 0)
                    {
                        _context.Update(transactionfee);
                    }
                    _context.Update(TransferableAccount);
                    _context.Update(transactionFrom);
                    _context.Update(transactionTo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferableAccountExists(TransferableAccount.id))
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

            return View("Index");

        }




        // GET: TransferableAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var TransferableAccount = await _context.Transferable.FindAsync(id);
            if (TransferableAccount == null || TransferableAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            return View(TransferableAccount);
        }

        // POST: TransferableAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,accId,UserName,Balance")] TransferableAccount TransferableAccount)
        {
            if (id != TransferableAccount.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(TransferableAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferableAccountExists(TransferableAccount.id))
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
            return View(TransferableAccount);
        }

        // GET: TransferableAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var TransferableAccount = await _context.Transferable
                .FirstOrDefaultAsync(m => m.id == id);
            if (TransferableAccount == null || TransferableAccount.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            if (TransferableAccount.Balance != 0)
            {
                return RedirectToAction("Index");
            }

            return View(TransferableAccount);
        }

        public async Task<IActionResult> ViewTransactions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            var transferableModel = await _context.Transferable
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

            var transferableModel = await _context.Transferable
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


            var transferableModel = await _context.Transferable
                .FirstOrDefaultAsync(m => m.id == id);
            ViewBag.id = transferableModel.id;
            if (transferableModel == null || transferableModel.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            var AccId = transferableModel.accId;
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
            return View("../ViewTransactions/ViewTransactions", TransacVM);
        }

        // POST: TransferableAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = new TransactionModel();
            var TransferableAccount = await _context.Transferable.FindAsync(id);

            transaction.accId = TransferableAccount.accId;
            transaction.transferTypeId = 5;
            transaction.Amount = 0;
            transaction.CreateTime = DateTime.Now;

            _context.Update(transaction);
            _context.Transferable.Remove(TransferableAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransferableAccountExists(int id)
        {
            return _context.Transferable.Any(e => e.id == id);
        }
    }
}
