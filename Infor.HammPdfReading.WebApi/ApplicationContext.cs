using Microsoft.EntityFrameworkCore;

namespace Infor.HammPdfReading.WebApi
{
    public class ApplicationContext : DbContext
    {
        public DbSet<WebModule> WebModules { get; set; }
        public DbSet<WebDetail> WebDetails { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
