using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetBankWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace NetBankWebApp.Models.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //public DbSet<UserManager<TUser>> User { get; set; }
        //public DbSet<CustomerModel> Customer { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasOne(p => p.Customer)
        //        .WithOne(b => b.User)
        //        .HasForeignKey(p => p.Username);
        //}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<CustomerModel> Customer { get; set; }
        public DbSet<CustomerDetailsModel> CustomerDetails { get; set; }
        public DbSet<TransactionModel> Transaction { get; set; }
        public DbSet<TransferableAccount> Transferable { get; set; }
        public DbSet<LoanModel> Loan { get; set; }
        public DbSet<TermDeposit> TermDeposit { get; set; }
    }
}
