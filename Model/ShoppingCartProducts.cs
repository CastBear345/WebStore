using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebStore.Model
{
    public class ShoppingCartProducts
    {
        [ForeignKey("ShoppingCart")]
        public int ShoppingCartId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        [JsonIgnore]

        public ShoppingCarts ShoppingCart { get; set; }
        [JsonIgnore]

        public Product Product { get; set; }
    }
}
