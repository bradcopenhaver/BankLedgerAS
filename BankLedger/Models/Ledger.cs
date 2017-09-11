using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedger.Models
{
    public class Ledger
    {
        public  List<Account> Accounts { get; set; }
        public  List<Transaction> Transactions { get; set; }
        public  bool Authenticated { get; set; }
        public  int CurrentAcctNum { get; set; }

        public Ledger()
        {
            Accounts = new List<Account> { };
            Transactions = new List<Transaction> { };
            Authenticated = false;
        }
    }
}
