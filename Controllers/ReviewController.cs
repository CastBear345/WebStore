using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model;

namespace WebStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationContext _context; // Замените WebStoreDbContext на ваш контекст базы данных

        public ReviewController(ApplicationContext context)
        {
            _context = context;

        }

        [HttpGet("{productId}/reviews")]
        public ActionResult<IEnumerable<Reviews>> GetProductReviews(int productId)
        {
            // Используйте контекст данных для получения отзывов для конкретного продукта
            var reviews = _context.Reviews.Where(r => r.ProductId == productId).ToList();

            return Ok(reviews);
        }
        [HttpPost("reviews")]
        public async Task<ActionResult<Reviews>> AddReview(Reviews review)
        {
            if (review == null)
            {
                return BadRequest("Отсутствуют данные о отзыве для добавления");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok("Отзыв успешно добавлен");
        }
    }
}
