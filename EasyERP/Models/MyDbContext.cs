using Microsoft.EntityFrameworkCore;

namespace EasyERP.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Product> products { get; set; }
        public DbSet<Order> orders { get; set; }

    }
}
