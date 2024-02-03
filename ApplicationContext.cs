using Microsoft.EntityFrameworkCore;
using WebStore.Model;

namespace WebStore
{
    public class ApplicationContext : DbContext
    {
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ListOfProducts> ListsOfProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }

        
        



        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.MainCategory)
                .WithMany(mc => mc.SubCategories)
                .HasForeignKey(sc => sc.MainCategoryId);

            modelBuilder.Entity<ShoppingCartProduct>()
                .HasKey(sp => new { sp.ShoppingCartId, sp.ProductId });

            modelBuilder.Entity<ShoppingCartProduct>()
                .HasOne(sp => sp.ShoppingCart)
                .WithMany(sc => sc.ShoppingCartProducts)
                .HasForeignKey(sp => sp.ShoppingCartId);

            modelBuilder.Entity<ShoppingCartProduct>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.ShoppingCartProducts)
                .HasForeignKey(sp => sp.ProductId);
        }
    }
}
