using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_EFC.Models;
using System.Reflection.Metadata;
using ASP_EFC.Helpers;

namespace ASP_EFC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.Customer).Include(o => o.Product);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData[Constants.CustomerId] = new SelectList(_context.Customers, "Id", "Name");
            ViewData[Constants.ProductId] = new SelectList(_context.Products, "Id", "Name");

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,TotalAmount,CustomerId,ProductId")] Order order)
        {
            var customer = await _context.Customers.FindAsync(order.CustomerId);
            var product = await _context.Products.FindAsync(order.ProductId);
            if (customer != null && product != null)
            {
                order.Customer = customer;
                order.Product = product;
            }

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Вывод ошибок в консоль
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }

            ViewData[Constants.CustomerId] = new SelectList(_context.Customers, "Id", "Email", order.CustomerId);
            ViewData[Constants.ProductId] = new SelectList(_context.Products, "Id", "Name", order.ProductId);

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData[Constants.CustomerId] = new SelectList(_context.Customers, "Id", "Email", order.CustomerId);
            ViewData[Constants.ProductId] = new SelectList(_context.Products, "Id", "Name", order.ProductId);

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,TotalAmount,CustomerId,ProductId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                return await OrderContexUpdateWithRedirectToAction(order);
            }
           
            ViewData[Constants.CustomerId] = new SelectList(_context.Customers, "Id", "Email", order.CustomerId);
            ViewData[Constants.ProductId] = new SelectList(_context.Products, "Id", "Name", order.ProductId);

            return View(order);
        }



        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private async Task<IActionResult> OrderContexUpdateWithRedirectToAction(Order order)
        {

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"Order with Id = {order.Id} not found");
                }
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
