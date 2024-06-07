using Business.Interfaces;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient.Server;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        // GET/api/receipts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetAll()
        {
            try
            {
                var response = await _receiptService.GetAllAsync();

                if (response != null) return Ok(response);
                else return NotFound(response);
            }
            catch { return NotFound(); }
        }

        // GET/api/receipts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            try
            {
                var response = await _receiptService.GetByIdAsync(id);

                if (response != null) return Ok(response);
                else return NotFound(response);
            }
            catch { return NotFound(); }
        }

        // GET/api/receipts/{id}/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetDetailsById(int id)
        {
            try
            {
                var response = await _receiptService.GetReceiptDetailsAsync(id);

                if (response != null) return Ok(response);
                else return NotFound(response);
            }
            catch { return NotFound(); }
        }

        // GET/api/receipts/{id}/sum
        [HttpGet("{id}/sum")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetSumById(int id)
        {
            try
            {
                var response = await _receiptService.ToPayAsync(id);

                return Ok(response);
            }
            catch { return BadRequest(); }
        }

        // GET/api/receipts/period?startDate=2021-12-1&endDate=2020-12-31
        [HttpGet("period/{startDate:DateTime?}/{endDate:DateTime?}")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> Get
            ([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                var formats = new[] { "yyyy-M-d", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d" };
                var culture = CultureInfo.InvariantCulture;
                if (DateTime.TryParseExact(startDate, formats, culture, DateTimeStyles.None, out var _startDate) &&
            DateTime.TryParseExact(endDate, formats, culture, DateTimeStyles.None, out var _endDate))
                {
                    var response = await _receiptService.GetReceiptsByPeriodAsync(_startDate, _endDate);

                    if (response != null && response.Any())
                        return Ok(response);
                    else
                        return NotFound();
                }
                else
                {
                    return BadRequest("Invalid date format. Please use yyyy-MM-dd format.");
                }
            }
            catch { return BadRequest(); }
        }

        // POST/api/receipts
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ReceiptModel value)
        {
            try
            {
                await _receiptService.AddAsync(value);
                return Ok(value);
            }
            catch { return BadRequest(); }
        }



        // PUT/api/receipts/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ReceiptModel value)
        {
            try
            {
                value.Id = id;

                await _receiptService.UpdateAsync(value);
                return Ok(value);
            }
            catch { return BadRequest(); }
        }

        // PUT/api/receipts/{id}/products/add/{productId}/{quantity}
        [HttpPut("{id}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult> AddProductToReceipt(int id, int productId, int quantity)
        {
            try
            {
                await _receiptService.AddProductAsync(productId, id, quantity);
                return Ok();
            }
            catch { return BadRequest(); }
        }

        // PUT/api/receipts/{id}/products/remove/{productId}/{quantity}
        [HttpPut("{id}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProductToReceipt(int id, int productId, int quantity)
        {
            try
            {
                await _receiptService.RemoveProductAsync(productId, id, quantity);
                return Ok();
            }
            catch { return BadRequest(); }
        }

        // PUT/api/receipts/{id}/checkout
        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> CheckoutReceipt(int id)
        {
            try
            {
                await _receiptService.CheckOutAsync(id);
                return Ok();
            }
            catch { return BadRequest(); }
        }



        //  DELETE/api/receipts/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _receiptService.DeleteAsync(id);
                return Ok();
            }
            catch { return Ok(); }
        }
    
    }
}
