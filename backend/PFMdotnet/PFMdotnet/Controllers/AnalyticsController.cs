using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Helpers.SearchReturnObjects.Analytics;
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
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}