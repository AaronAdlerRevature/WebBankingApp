using System;
using System.Collections.Generic;
using System.Text;

namespace NetBankWebApp.Models.Models
{
    public class Transfer
    {
        public int fromId { get; set; }
        public int toId { get; set; }
        public string fromAccId { get; set; }
        public decimal Amount { get; set; }
        public virtual TransferableAccount toAcc { get; set; }
    }
}
