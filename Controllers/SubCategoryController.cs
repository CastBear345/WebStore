using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Model;
using WebStore.ModelDTO;

namespace WebStore.Controllers
{
    [Route("subcategories")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SubCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{subCategoryId}")]
        public IActionResult DeleteSubCategory(int subCategoryId)
        {
            var subcategoryToDelete = _context.SubCategory.FirstOrDefault(s => s.Id == subCategoryId);

            if (subcategoryToDelete == null)
            {
                return NotFound();
            }

            _context.SubCategory.Remove(subcategoryToDelete);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{subCategoryId}")]
        public IActionResult UpdateSubcategory(int subCategoryId, SubCategoryUpdateDto subCategory)
        {
            var subcategoryToUpdate = _context.SubCategory.FirstOrDefault(s => s.Id == subCategoryId);
            var doesMainCategoryIdExist = _context.MainCategory.Any(c => c.Id == subCategory.MainCategoryId);

            if (subcategoryToUpdate == null || !doesMainCategoryIdExist)
            {
                return NotFound();
            }

            subcategoryToUpdate.Name = subCategory.Name;
            subcategoryToUpdate.IconURL = subCategory.IconURL;
            subcategoryToUpdate.ImageURL = subCategory.ImageURL;
            subcategoryToUpdate.MainCategoryId = subCategory.MainCategoryId;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public ActionResult CreateSubCategory(SubCategoryCreationDto subCategory)
        {
            var doesMainCategoryIdExist = _context.MainCategory.Any(c => c.Id == subCategory.MainCategoryId);

            if (!doesMainCategoryIdExist)
            {
                return NotFound();
            }

            var newSubcategory = new SubCategory()
            {
                Name = subCategory.Name,
                MainCategoryId = subCategory.MainCategoryId,
                ImageURL = subCategory.ImageURL,
                IconURL = subCategory.IconURL,
            };

            _context.SubCategory.Add(newSubcategory);
            _context.SaveChanges();

            return Ok();
        }
    }
}
