using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetBankWebApp.Models.Models;
using System;
using System.Collections.Generic;

namespace NetBankWebApp.BusinessLayer
{
    public class TransacVM
    {
        private readonly ApplicationDbContext _context;

        public TransacVM(ApplicationDbContext context)
        {
            _context = context;
        }

        static void getTransacType(List<TransactionVM> vm)
        {
            foreach (var item in vm)
            {
                switch (item.transferTypeId)
                {
                    case 0:
                        item.typeName = "Create";
                        break;
                    case 1:
                        item.typeName = "Deposit";
                        break;
                    case 2:
                        item.typeName = "Withdraw";
                        break;
                    case 3:
                        item.typeName = "Transfer";
                        break;
                    case 4:
                        item.typeName = "Fee";
                        break;
                    case 5:
                        item.typeName = "Close";
                        break;
                }
            }
        }
            
    }
}
