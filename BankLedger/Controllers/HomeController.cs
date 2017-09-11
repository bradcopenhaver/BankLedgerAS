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
                ViewBag.Message = "No accounts in ledger. Please create a new account.";

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));

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

                    ViewBag.Message = "Account created. Log in to make an initial deposit.";
                }
                else
                {
                    ViewBag.Message = "Passwords do not match. Try again.";
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
                Account currentAcct = cachedLedger.Accounts.Find(x => x.AcctNumber == loginAcctNum);
                //See if password matches
                if(currentAcct.Password == loginPswd)
                {
                    //"Login" to account and update cache
                    cachedLedger.Authenticated = true;
                    cachedLedger.CurrentAcctNum = currentAcct.AcctNumber;

                    _cache.Set("cachedLedger", cachedLedger);

                    return RedirectToAction("Account", new { id = currentAcct.AcctNumber });
                }
                else
                {
                    ViewBag.Message = "Incorrect password.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Message = "That account number does not exist.";
                return RedirectToAction("Index");
            }
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
                Account currentAcct = cachedLedger.Accounts.Find(x => x.AcctNumber == cachedLedger.CurrentAcctNum);

                ViewBag.CurrentAcct = currentAcct;
                return View();
            }
            else
            {
                ViewBag.Message = "You are not logged in.";
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

            //Update cache
            _cache.Set("cachedLedger", cachedLedger);

            return RedirectToAction("Account"); //This doesn't work yet.
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
