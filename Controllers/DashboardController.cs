﻿using Expence_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expence_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _Context;

        public DashboardController(AppDbContext appDbContext)
        {
            _Context = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            //Last 7 Dayes
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Now;

            var SelectedTransactions = await _Context.Transactions.Include(c=>c.Category)
                .Where(c=>c.Date>=StartDate & c.Date<=EndDate).ToListAsync();
                
            //TotalIncome
            var TotalIncome = SelectedTransactions.Where(i => i.Category.Type == "Income")
                .Sum(t => t.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //TotalExpense
            var TotalExpense = SelectedTransactions.Where(i => i.Category.Type == "Expense")
                .Sum(t => t.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance
            var Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

            //Duaghnut chart and splint chart
            ViewBag.DuaghnutchartData =  SelectedTransactions.Where(t => t.Category.Type == "Expense")
                .GroupBy(i => i.CategoryID).Select(j => new
                {
                    ctegoryTitleWithIcon = j.First().Category.Icone+" "+ j.First().Category.Title,
                    amount = j.Sum(k => k.Amount),
                    formattedAmount = j.Sum(k => k.Amount).ToString("C0")
                }).OrderByDescending(l => l.amount).ToList();


            //Spline Chart - Income vs Expense

            //Income
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            //Combine Income & Expense
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };
            //Recent Transactions
            ViewBag.RecentTransactions = await _Context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();
            return View();
        }
    }
    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;

    }
}
