﻿using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Commands;
using PFMdotnet.Database.Enums;
using PFMdotnet.Helpers.ParseCSV;
using PFMdotnet.Models;
using PFMdotnet.Services;

namespace PFMdotnet.Controllers
{

    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {

        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] SearchTransactionParams searchParams)
        {

            var result = await _transactionService.GetTransactionsAsQueriable(searchParams);

            if (result.Errors != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTransaction([FromRoute] string id)
        {

            var result = await _transactionService.GetTransaction(id);

            if (result.Errors != null)
            {
                return StatusCode(404, result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportTransactions(IFormFile file)
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
                return BadRequest(new
                {
                    Message = "Uploading CSV file",
                    Error = "The file is not in CSV format.",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var res = await _transactionService.CreateTransactionBulk(CsvManip.ToList<CreateTransactionCommand>(file));

            return Ok(res);
        }

        [HttpPost]
        [Route("{id}/categorize")]
        public async Task<IActionResult> CategorizeTransaction([FromRoute] string id, [FromQuery] string? catCode)
        {            
            var result = await _transactionService.AddCategoryToTransaction(id, catCode);

            if (result.Errors != null)
            {
                return NotFound(result);
            }

            return Ok(result);   

        }

        [HttpPost]
        [Route("categorize")]
        public async Task<IActionResult> CategorizeManyTransactions([FromQuery] string? ids, [FromQuery] string? catCode)
        {

            var result = await _transactionService.AddCategoryToManyTransactions(ids, catCode);

            if (result.Errors != null)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost("{id}/split")]
        public async Task<IActionResult> SplitTransaction([FromRoute] string id, [FromBody] SplitByParams splitParams)
        {

            var result = await _transactionService.SplitTransactionAsync(id, splitParams);

            if (result.Errors != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("auto-categorize")]
        public async Task<IActionResult> AutoCategorize()
        {

            var result = await _transactionService.AutoCategorize();

            if (result.Error != null)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



    }
}
