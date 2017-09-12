using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankLedger.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BankLedger.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _cache;

        public HomeController(IMemoryCache memoryCache)
        {
            //Initialize cache
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            Ledger cachedLedger = new Ledger();

            // Check if cached ledger exists already.
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                // Key not in cache, so make a new ledger.
                Dictionary<string, object> newCachedLedger = new Dictionary<string, object> { };
                Ledger newLedger = new Ledger();
                
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache.
                    .SetPriority(CacheItemPriority.NeverRemove);

                // Save data in cache.
                _cache.Set("cachedLedger", newLedger, cacheEntryOptions);
            }

            _cache.TryGetValue("cachedLedger", out cachedLedger);
            ViewBag.Ledger = cachedLedger;

            return View();
        }

        [HttpPost]
        public IActionResult Create(int newAcctNum, string pswd, string confPswd)
        {
            //Retrieve cached ledger
            Ledger cachedLedger = new Ledger();
            if (_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                if (pswd == confPswd)
                {
                    Account newAcct = new Account(newAcctNum, pswd);
                    cachedLedger.Accounts.Add(newAcct);
                    cachedLedger.MessageIndex = 2;
                }
                else
                {
                    cachedLedger.MessageIndex = 3;
                }                
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Login(int loginAcctNum, string loginPswd)
        {
            //Retrieve cached ledger
            Ledger cachedLedger = new Ledger();
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                return RedirectToAction("Index");
            }

            //See if account exists
            if(cachedLedger.Accounts.Exists(x => x.AcctNumber==loginAcctNum))
            {
                //Retrieve account
                Account currentAcct = cachedLedger.Accounts.Find(x => x.AcctNumber == loginAcctNum);

                //See if password matches
                if(currentAcct.Password == loginPswd)
                {
                    //"Login" to account and update cache
                    cachedLedger.Authenticated = true;
                    cachedLedger.CurrentAcctNum = currentAcct.AcctNumber;
                    
                    return RedirectToAction("Account", new { id = currentAcct.AcctNumber });
                }
                else
                {
                    cachedLedger.MessageIndex = 4;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                cachedLedger.MessageIndex = 5;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Logout()
        {
            //Retrieve cached ledger
            Ledger cachedLedger = new Ledger();
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                return RedirectToAction("Index");
            }

            //Update cache
            cachedLedger.Authenticated = false;
            cachedLedger.MessageIndex = 7;

            return RedirectToAction("Index");
        }

        public IActionResult Account(string id)
        {
            //Retrieve cached ledger
            Ledger cachedLedger = new Ledger();
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                return RedirectToAction("Index");
            }

            //Check authentication
            if (cachedLedger.Authenticated == true && cachedLedger.CurrentAcctNum == Int32.Parse(id))
            {
                //Retrieve account
                Account currentAcct = cachedLedger.Accounts.Find(x => x.AcctNumber == cachedLedger.CurrentAcctNum);

                //Create list of account's transactions
                List<Transaction> currentAcctTransactions = cachedLedger.Transactions.FindAll(x => x.AccountNumber == cachedLedger.CurrentAcctNum);

                ViewBag.Transactions = currentAcctTransactions;
                ViewBag.CurrentAcct = currentAcct;
                return View();
            }
            else
            {
                cachedLedger.MessageIndex = 6;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Deposit(int depAcctNum, double depAmt)
        {
            //Retrieve cached ledger
            Ledger cachedLedger = new Ledger();
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                return RedirectToAction("Index");
            }
            
            //Retrieve account
            Account currentAcct = cachedLedger.Accounts.Find(x => x.AcctNumber == depAcctNum);

            //Apply deposit
            double startBal = currentAcct.Balance;
            currentAcct.Balance += depAmt;
            double endBal = currentAcct.Balance;

            //Log transaction
            cachedLedger.Transactions.Add(new Transaction(depAcctNum, startBal, endBal));
            
            return RedirectToAction("Account", new { id = cachedLedger.CurrentAcctNum }); 
        }

        [HttpPost]
        public IActionResult Withdraw(int wdAcctNum, double wdAmt)
        {
            //Retrieve cached ledger
            Ledger cachedLedger = new Ledger();
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
            {
                return RedirectToAction("Index");
            }

            //Retrieve account
            Account currentAcct = cachedLedger.Accounts.Find(x => x.AcctNumber == wdAcctNum);

            //Apply withdraw
            double startBal = currentAcct.Balance;
            currentAcct.Balance -= wdAmt;
            double endBal = currentAcct.Balance;

            //Log transaction
            cachedLedger.Transactions.Add(new Transaction(wdAcctNum, startBal, endBal));
            
            return RedirectToAction("Account", new { id = cachedLedger.CurrentAcctNum });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
