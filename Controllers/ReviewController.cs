using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swagger.Models;
using System.Net;
using WebStore.Model;

namespace WebStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    protected APIResponse _response;

    public ReviewController(ApplicationDbContext context)
    {
        _context = context;
        this._response = new();
    }

    [Authorize]
    [HttpGet("{productId}/reviews")]
    public ActionResult<IEnumerable<Reviews>> GetProductReviews(int productId)
    {
        // Используйте контекст данных для получения отзывов для конкретного продукта
        var reviews = _context.Reviews.Where(r => r.ProductId == productId).ToList();

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = reviews;
        return Ok(_response);
    }

    [Authorize]
    [HttpPost("{productId}/reviews")]
    public async Task<ActionResult<Reviews>> AddReview(int productId, Reviews review)
    {
        if (review == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = true;
            _response.ErrorMessages.Add("Отсутствуют данные о отзыве для добавления");
            return BadRequest(_response);
        }

        review.ProductId = productId; // Устанавливаем идентификатор продукта

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductReviews), new { productId }, review);
    }

    [Authorize]
    [HttpDelete("{productId}/reviews/{reviewId}")]
    public async Task<ActionResult> DeleteReview(int productId, int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);

        if (review == null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = true;
            return NotFound(_response);
        }

        if (review.ProductId != productId)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = true;
            _response.ErrorMessages.Add("Идентификатор продукта не соответствует существующему отзыву");
            return BadRequest(_response);
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        return Ok(_response);
    }

    [Authorize]
    [HttpPut("{productId}/reviews/{reviewId}")]
    public async Task<ActionResult<Reviews>> UpdateReview(int productId, int? reviewId, Reviews updatedReview)
    {
        if (updatedReview == null || reviewId == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = true;
            return BadRequest(_response);
        }

        var existingReview = await _context.Reviews.FindAsync(reviewId);

        if (existingReview == null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = true;
            return NotFound(_response);
        }

        existingReview.Name = updatedReview.Name;
        existingReview.Content = updatedReview.Content;
        existingReview.Grade = updatedReview.Grade;
        existingReview.ProductId = productId;

        await _context.SaveChangesAsync();

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = existingReview;
        return Ok(_response);
    }
}
