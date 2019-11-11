using Microsoft.EntityFrameworkCore;
using NetBankWebApp.Models.Models;
using NetBankWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBankWebApp.Models.Repos
{
    public class CustomerRepo
    {
        private ApplicationDbContext _context;

        public CustomerRepo(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<CustomerModel> Get(string Username)
        {
            var Customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.Username == Username);

            return Customer;
        }

        public async Task<List<CustomerModel>> Get()
        {
            return await _context.Customer.ToListAsync();

        }

        public bool CustomerModelExists(string Username)
        {
            return _context.Customer.Any(e => e.Username == Username);
        }

        public async Task<bool> Create(CustomerModel Customer)
        {
            _context.Add(Customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(int? id, CustomerModel Customer)
        {
            _context.Update(Customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteConfirmed(int id)
        {
            var Customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(Customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
