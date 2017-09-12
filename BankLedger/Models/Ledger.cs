using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedger.Models
{
    public class Ledger
    {
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public bool Authenticated { get; set; }
        public int CurrentAcctNum { get; set; }
        public string[] Messages { get; set; }
        public int MessageIndex { get; set; }

        public Ledger()
        {
            Accounts = new List<Account> { };
            Transactions = new List<Transaction> { };
            Authenticated = false;
            Messages = new string[] 
            {
                "",
                "No accounts in ledger. Please create a new account.",
                "Account created. Log in to make an initial deposit.",
                "Passwords do not match. Try again.",
                "Incorrect password.",
                "That account number does not exist.",
                "You are not logged in to the account you tried to access.",
                "You have logged out."
            };
            MessageIndex = 1;
        }
    }
}
