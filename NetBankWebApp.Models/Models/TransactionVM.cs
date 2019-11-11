using System;
using System.Collections.Generic;
using System.Text;

namespace NetBankWebApp.Models.Models
{
    public class TransactionVM
    {
        public int id                     {get; set;}
        public int transferTypeId         {get; set;}
        public string typeName            {get; set;}
        public string accId               { get; set; }
        public string toAccId                {get; set;}
        public decimal Amount             {get; set;}
        public DateTime CreateTime        { get; set; }

        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
    }
}
