using System.ComponentModel.DataAnnotations;

namespace WebStore.Model
{
    public class MainCategory
    {
        [Key]
        public int MainCategoryId { get; set; }

        [Required]
        public string MainCategoryName { get; set; }

        public List<SubCategory> SubCategories { get; set; }
    }
}
