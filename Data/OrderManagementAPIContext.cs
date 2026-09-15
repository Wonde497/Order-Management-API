using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Data
{
    public class OrderManagementAPIContext : DbContext
    {
        public OrderManagementAPIContext(DbContextOptions<OrderManagementAPIContext> options) 
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users {get;set;}
        public DbSet<Product> Products{get;set;}
        public DbSet<Order> Orders{get;set;}
        public DbSet<OrderItem> OrderItems{get;set;}
        
    }
} 
