using Microsoft.EntityFrameworkCore;
using WebStore.Model;

namespace WebStore
{
    public class ApplicationContext : DbContext
    {
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ListsOfProducts> ListsOfProducts { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<ShoppingCarts> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartProducts> ShoppingCartProducts { get; set; }






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

            modelBuilder.Entity<ShoppingCartProducts>()
                .HasKey(sp => new { sp.ShoppingCartId, sp.ProductId });

            modelBuilder.Entity<ShoppingCartProducts>()
                .HasOne(sp => sp.ShoppingCart)
                .WithMany(sc => sc.ShoppingCartProducts)
                .HasForeignKey(sp => sp.ShoppingCartId);

            modelBuilder.Entity<ShoppingCartProducts>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.ShoppingCartProducts)
                .HasForeignKey(sp => sp.ProductId);


            //modelBuilder.Entity<Product>()
            //    .HasOne(p => p.SubCategory)
            //    .WithMany();

            //modelBuilder.Entity<Product>()
            //    .HasOne(p => p.ListOfProducts)
            //    .WithMany();
            //modelBuilder.Entity<Product>()
            //    .HasMany(p => p.Reviews)
            //    .WithOne();

            //modelBuilder.Entity<Product>()
            //    .HasMany(p => p.ShoppingCartProducts)
            //    .WithOne();

        }
    }
}
