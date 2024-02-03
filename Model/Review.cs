using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Model
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public int Grade { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
