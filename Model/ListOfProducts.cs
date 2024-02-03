using System.ComponentModel.DataAnnotations;

namespace WebStore.Model
{
    public class ListOfProducts
    {
        [Key]
        public int ListOfProductsId { get; set; }

        public List<Product> Products { get; set; }
    }
}
