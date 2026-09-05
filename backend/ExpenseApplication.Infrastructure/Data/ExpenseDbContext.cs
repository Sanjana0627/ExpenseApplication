using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseApplication.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApplication.Infrastructure.Data
{ 
    public class ExpenseDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<ExpenseForm> ExpenseForms => Set<ExpenseForm>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ExpenseFormHistory> ExpenseFormHistories => Set<ExpenseFormHistory>();
        public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();
        // sets up table relationships,primary keys,foregin keys and adds the fixed Role and Currency rows
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the relationships and constraints
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany(m => m.DirectReports)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpenseForm>()
                .HasOne(f => f.Employee)
                .WithMany()
                .HasForeignKey(f => f.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpenseForm>()
                .HasOne(f => f.Manager)
                .WithMany()
                .HasForeignKey(f => f.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpenseFormHistory>()
                .HasOne(h => h.PerformedByUser)
                .WithMany()
                .HasForeignKey(h => h.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpenseForm>().HasKey(f => f.FormId);
            modelBuilder.Entity<ExpenseFormHistory>().HasKey(h => h.HistoryId);
            modelBuilder.Entity<Expense>().Property(e => e.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<ExpenseForm>().Property(f => f.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Employee" },
                new Role { RoleId = 2, RoleName = "Manager" },
                new Role { RoleId = 3, RoleName="Accountant" },
                new Role { RoleId = 4, RoleName = "Admin" }
            );
            modelBuilder.Entity<Currency>().HasData(
                new Currency { CurrencyId = 1, Code = "TL" },
                new Currency { CurrencyId = 2, Code = "EUR" },
                new Currency { CurrencyId = 3, Code = "USD" },
                new Currency { CurrencyId = 4, Code = "PKR" },
                new Currency { CurrencyId = 5, Code = "INR" },
                new Currency { CurrencyId = 6, Code = "AED" }
            );
            modelBuilder.Entity<ErrorLog>().HasKey(e => e.LogId);

        }
    }
}
