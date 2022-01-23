using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Shopping.Persistance.ReadStore.DbContext
{
    public class OrderDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly IConfiguration configuration;
        public OrderDbContext()
        {

        }
        public OrderDbContext(DbContextOptions<OrderDbContext> options, IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public DbSet<OrderDetailView> OrderDetailView { get; set; }
        public DbSet<OrderItemView> OrderItemView { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetailView>()
               .HasMany(x => x.OrderItemViews)
               .WithOne(q => q.OrderDetailView)
               .HasForeignKey(q => q.OrderId);

       

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("ReadStoreConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

       
    }
}
