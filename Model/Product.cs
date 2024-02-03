using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Model
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageURL { get; set; }

        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; }

        [ForeignKey("ListOfProducts")]
        public int ListOfProductsId { get; set; }

        public ListOfProducts ListOfProducts { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int CountOfLikes { get; set; }

        public List<Review> Reviews { get; set; }

        public List<ShoppingCartProduct> ShoppingCartProducts { get; set; }
    }
}
