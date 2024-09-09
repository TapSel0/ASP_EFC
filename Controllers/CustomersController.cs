using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_EFC.Models;
using System.Xml.Linq;

namespace ASP_EFC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;

            //ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "NameDesc" : "";
            //ViewData["EmailSortParm"] = sortOrder == "EmailAsc" ? "EmailDesc" : "EmailAsc";
            //ViewData["PhoneSortParm"] = sortOrder == "PhoneAsc" ? "PhoneDesc" : "PhoneAsc";

            var customers = from c in _context.Customers select c;

            // Логика сортировки
            switch (sortOrder)
            {
                case "NameDesc":
                    customers = customers.OrderByDescending(c => c.Name);
                    break;
                case "NameAsc":
                    customers = customers.OrderBy(c => c.Name);
                    break;
                case "EmailDesc":
                    customers = customers.OrderByDescending(c => c.Email);
                    break;
                case "EmailAsc":
                    customers = customers.OrderBy(c => c.Email);
                    break;
                case "PhoneDesc":
                    customers = customers.OrderByDescending(c => c.PhoneNumber);
                    break;
                case "PhoneAsc":
                    customers = customers.OrderBy(c => c.PhoneNumber);
                    break;
                default:
                    customers = customers.OrderBy(c => c.Name);
                    break;
            }

            return View(await customers.AsNoTracking().Include(c => c.Orders).ThenInclude(o => o.Product).ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,PhoneNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);

            //if (!ModelState.IsValid)
            //{
            //    // Вывод ошибок в консоль
            //    foreach (var state in ModelState)
            //    {
            //        foreach (var error in state.Value.Errors)
            //        {
            //            Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
            //        }
            //    }
            //return View(customer);
        //     }
        //_context.Add(customer);
        //await _context.SaveChangesAsync();
        //return RedirectToAction(nameof(Index));
    }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        // Example of filter
        public async Task<IActionResult> FilteredCustomers(string name)
        {
            var filteredCustomers = await _context.Customers
                .Where(c => c.Name.StartsWith(name))
                .ToListAsync();

            return View("Index", filteredCustomers);
        }

        // Example of sorting
        public async Task<IActionResult> SortedCustomers()
        {
            var sortedCustomers = await _context.Customers
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View("Index", sortedCustomers);
        }

        public async Task<IActionResult> FilterAmountOfOrders(float amount)
        {
            var filteredCustomers = from c in _context.Customers
                                    join o in _context.Orders on c.Id equals o.CustomerId
                                    join p in _context.Products on o.ProductId equals p.Id
                                    select new CustomerOrderViewModel
                                    {
                                        CustomerName = c.Name,
                                        ProductName = p.Name,
                                        TotalPurchaseAmount = o.TotalAmount * p.Price
                                    };
            return View(filteredCustomers.ToList());
        }

        // Sampling with projection:
        //    var customerNames = await context.Customers
        //      .Select(c => c.Name)
        //      .ToListAsync();

        // Retrieving data using Include:
        // var customersWithOrders = await context.Customers
        //      .Include(c => c.Orders)
        //      .ToListAsync();

        //Performing data aggregation:
        // var customerCount = await context.Customers.CountAsync();

        // When you have nested collections, for example a customer has orders and orders have products, you need to use ThenInclude:
        // var customersWithOrdersAndProducts = await context.Customers
        //      .Include(c => c.Orders)
        //      .ThenInclude(o => o.Products)
        //      .ToListAsync();

    }
}
