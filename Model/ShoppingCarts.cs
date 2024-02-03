using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebStore.Model
{
    public class ShoppingCarts
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal TotalSumOfPrices { get; set; }

        [JsonIgnore]
        public List<ShoppingCartProducts> ShoppingCartProducts { get; set; }
    }
}
