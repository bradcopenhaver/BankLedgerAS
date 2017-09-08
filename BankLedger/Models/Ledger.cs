using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedger.Models
{
    public class Ledger
    {
        public static List<Account> Accounts { get; set; }
        public static List<Transaction> Transactions { get; set; }
        public static bool Authenticated { get; set; }
        public static int CurrentAcctNum { get; set; }

        public Ledger()
        {
            Authenticated = false;
        }
    }
}
