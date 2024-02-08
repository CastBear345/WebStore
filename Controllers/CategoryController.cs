using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model;
using WebStore.ModelDTO;

namespace WebStore.Controllers;

[ApiController]
[Route("categories")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult GetCateogiries()
    {
        return Ok(_context.MainCategory
            .Include(p => p.SubCategory)
            .ToList());
    }

    [HttpPost]
    public ActionResult AddCategory(CategoryCreationDto category)
    {
        var newCategory = new MainCategory()
        {
            Name = category.Name,
        };

        _context.MainCategory.Add(newCategory);
        _context.SaveChanges();

        return Created();
    }

    [HttpPut("{categoryId}")]
    public ActionResult UpdateCategory(CategoryUpdateDto category, int categoryId)
    {
        var categoryToUpdate = _context.MainCategory.FirstOrDefault(c => c.Id == categoryId);

        if (categoryToUpdate == null)
        {
            return NotFound();
        }

        categoryToUpdate.Name = category.Name;

        _context.SaveChanges();
        return Created();
    }
}
