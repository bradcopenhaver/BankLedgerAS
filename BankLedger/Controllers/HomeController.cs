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

            // Look for cache key.
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
            Ledger cachedLedger = new Ledger();
            if (!_cache.TryGetValue("cachedLedger", out cachedLedger))
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
            //else
            //{
            //    ViewBag.Message = "No accounts in ledger.Please create a new account.";
            //}
            return RedirectToAction("Index");
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
