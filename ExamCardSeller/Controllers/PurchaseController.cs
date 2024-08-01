using ExamCardSeller.ServiceModels;
using ExamCardSeller.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamCardSeller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController (IPurchaseService purchaseService) : ControllerBase
    {

        // GET api/<PurchaseController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var response = await purchaseService.VerifyOrder(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateVerificationRequest request)
        {
            var response = await purchaseService.MakeOrder(request);
            return Ok(response);
        }

    }
}
