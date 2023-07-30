using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Commands;
using PFMdotnet.Helpers.ParseCSV;
using PFMdotnet.Helpers.SearchReturnObjects.Categories;
using PFMdotnet.Models;
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
        public async Task<IActionResult> ImportCategories(IFormFile? file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Message = "Uploading CSV file",
                    Error = "No file given. Please upload a CSV file.",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            if (!CsvManip.IsValidCsvFile(file))
            {
                return StatusCode(403, new
                {
                    Message = "Uploading CSV file",
                    Error = "The file is not in CSV format.",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var res = await _categoryService.CreateCategoryBulk(CsvManip.ToList<CreateCategoryCommand>(file));

            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] SearchCategoriesParams searchParams)
        {

            var result = await _categoryService.GetCategoriesAsQueriable(searchParams);

            if (result.Errors != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
    }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCategory([FromRoute] string? id)
        {

            var result = await _categoryService.GetCategory(id);

            if (result.Errors != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


    }
}

