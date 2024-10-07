using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_EFC.Models;
using ASP_EFC.ViewModels;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

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
                return await CustomerContexUpdateWithRedirectToAction(customer);
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
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
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

        public IActionResult FilterAmountOfOrders(decimal amount)
        {
            var filteredCustomers = _context.Customers
                .Join(_context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { c, o })
                .Join(_context.Products, co => co.o.ProductId, p => p.Id, (co, p) => new
                {
                    co.c,
                    co.o,
                    Product = p,
                    TotalPurchaseAmount = co.o.TotalAmount * p.Price
                })
                .Where(result => result.TotalPurchaseAmount > amount)
                .Select(result => new CustomerOrderViewModel
                {
                    CustomerName = result.c.Name,
                    ProductName = result.Product.Name,
                    TotalPurchaseAmount = result.TotalPurchaseAmount
                })
                .ToList();

            return View(filteredCustomers);
        }

        [HttpGet]
        public async Task<IActionResult> GroupedByOrders()
        {
            var customerOrders = _context.Orders
                .Include(o => o.Product)
                .GroupBy(o => o.CustomerId)
                .Select(group => new
                {
                    CustomerId = group.Key,
                    TotalPurchaseAmount = group.Sum(o => o.TotalAmount * o.Product.Price)
                })
                .ToList();

            return View(customerOrders);
        }


        public async Task<IActionResult> FilterByTotalPrice()
        {
            var filteredCustomers = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .Where(c => c.Orders.Sum(o => o.TotalAmount * o.Product.Price) < 500)
                .ToListAsync();
            
            return View("Index", filteredCustomers);
        }

        public async Task<IActionResult> FilterByOrdersDate()
        {
            var customersWithOrdersThisMonth = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .Where(c => c.Orders.Any(o => o.OrderDate.Month == DateTime.Now.Month && o.OrderDate.Year == DateTime.Now.Year))
                .ToListAsync();

            return View("Index", customersWithOrdersThisMonth);
        }


        public async Task<IActionResult> GroupBy5Orders()
        {
            var customersWith5Orders = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .Where(c => c.Orders.Count >= 5)
                .AsNoTracking()
                .ToListAsync();

            return View("Index", customersWith5Orders);
        }

        private async Task<IActionResult> CustomerContexUpdateWithRedirectToAction(Customer customer)
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
                    throw new Exception($"Customer with Id = {customer.Id} not found");
                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
