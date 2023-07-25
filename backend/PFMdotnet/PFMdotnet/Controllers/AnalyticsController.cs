using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Models.Analytics;
using PFMdotnet.Services;

namespace PFMdotnet.Controllers
{

    [ApiController]
    public class AnalyticsController : ControllerBase
    {

        private readonly ICategoryService _categoryService;

        public AnalyticsController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("spending-analytics")]
        public async Task<IActionResult> SpendingAnalytics([FromQuery] AnalyticsSearchParams searchParams)
        {
            

            var result = await _categoryService.GetAnalytics(searchParams);

            if (result.Errors != null)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}