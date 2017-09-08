using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedger.Models
{
    public class Account
    {
        public int AcctNumber { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }

        public Account(int acctNum, string pswd, double bal)
        {
            AcctNumber = acctNum;
            Password = pswd;
            Balance = bal;
        }

        public void Deposit(float depositAmt)
        {
            Balance = Balance + depositAmt;
        }

        public void Withdraw(float withdrawAmt)
        {
            Balance = Balance - withdrawAmt;
        }
    }
}
