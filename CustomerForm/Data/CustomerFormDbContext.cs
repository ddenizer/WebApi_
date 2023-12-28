using CustomerForm.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerForm.Data
{
    public class CustomerFormDbContext : DbContext
    {
        public CustomerFormDbContext(DbContextOptions<CustomerFormDbContext> options)
            : base(options)
        {
        }
        public DbSet<CustomerFormTbl> CustomerForm { get; set; }       
    }
}
