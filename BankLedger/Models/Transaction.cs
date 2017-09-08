using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedger.Models
{
    public class Transaction
    {
        public int AccountNumber { get; set; }
        public double StartingBalance { get; set; }
        public double EndingBalance { get; set; }
        public DateTime TimeOfTransaction { get; set; }

        public Transaction(int acctNum, double startBal, double endBal)
        {
            AccountNumber = acctNum;
            StartingBalance = startBal;
            EndingBalance = endBal;
            TimeOfTransaction = DateTime.Now;
        }
    }
}
