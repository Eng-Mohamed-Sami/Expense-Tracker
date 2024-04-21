using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Expence_Tracker.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options):base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
