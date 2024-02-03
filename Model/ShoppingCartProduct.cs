using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Model
{
    public class ShoppingCartProduct
    {
        [ForeignKey("ShoppingCart")]
        public int ShoppingCartId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        public Product Product { get; set; }
    }
}
