using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PFMdotnet.Commands;
using PFMdotnet.Database;
using PFMdotnet.Services;
using System.Globalization;

namespace PFMdotnet.Controllers
{
    [ApiController]
    [Route("api/csv")]
    public class CsvController : ControllerBase
    {

        private readonly ITransactionService _transactionService;

        public CsvController(TransactionDbContext dbContext, ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        [HttpPost]
        public async Task<IActionResult> ImportTransactions()
        {

            string filePath = "C:\\Users\\koki_\\Desktop\\asseco\\proektFiles\\transactions.csv";

            if (filePath == null || filePath.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            
            using (var streamReader = new StreamReader(filePath))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                                       
                    var records = csvReader.GetRecords<CreateTransactionCommand>().ToList();

                    await _transactionService.CreateTransactionBulk(records);
                    
                }
            }

            return Ok("Transactions imported successfully.");
        }
    }
}