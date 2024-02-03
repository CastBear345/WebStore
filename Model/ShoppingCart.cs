using System.ComponentModel.DataAnnotations;

namespace WebStore.Model
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public decimal TotalSumOfPrices { get; set; }

        public List<ShoppingCartProduct> ShoppingCartProducts { get; set; }
    }
}
