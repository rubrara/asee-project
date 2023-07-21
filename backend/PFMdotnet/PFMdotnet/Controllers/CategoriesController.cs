using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Commands;
using PFMdotnet.Helpers.ParseCSV;
using PFMdotnet.Services;

namespace PFMdotnet.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportCategories(IFormFile file)
        {
            await _categoryService.CreateCategoryBulk(CsvParse<CreateCategoryCommand>.ToList(file));

            return Ok("Transactions imported successfully.");
        }
        

        
    }
}

