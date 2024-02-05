using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebStore.ModelDTO
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public int SubCategoryId { get; set; }
        public int ListOfProductsId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CountOfLikes { get; set; }
    }
}
