using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Model
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageURL { get; set; }
        public string IconURL { get; set; }

        [ForeignKey("MainCategory")]
        public int MainCategoryId { get; set; }
        public List<Product> Products { get; set; }

        public MainCategory MainCategory { get; set; }


    }
}
