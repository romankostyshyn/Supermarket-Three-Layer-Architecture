﻿using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Globalization;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;


        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }



        // GET/api/statistic/popularProducts?productCount=2
        [HttpGet("popularProducts/")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostPopular([FromQuery] int productCount)
        {
            try
            {
                var response = await _statisticService.GetMostPopularProductsAsync(productCount);

                if (response != null) return Ok(response);
                else return NotFound(response);
            }
            catch { return BadRequest(); }
        }


        // GET/api/statisic/customer/{id}/{productCount}
        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostPopularOfCustomer
            (int id, int productCount)
        {
            try
            {
                var response = await _statisticService.GetCustomersMostPopularProductsAsync(productCount, id);

                if (response != null) return Ok(response);
                else return NotFound(response);
            }
            catch { return BadRequest(); }
        }

        //  GET/api/statistic/activity/{customerCount}?startDate= 2020-7-21&endDate= 2020-7-22
        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostActiveCustomer
    (int customerCount, [FromQuery] string startDate = null, [FromQuery] string endDate = null)
        {
            try
            {
                var formats = new[] { "yyyy-M-d", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d" };
                var culture = CultureInfo.InvariantCulture;

                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    return BadRequest("Start date and end date are required.");
                }

                if (DateTime.TryParseExact(startDate, formats, culture, DateTimeStyles.None, out var _startDate) &&
                    DateTime.TryParseExact(endDate, formats, culture, DateTimeStyles.None, out var _endDate))
                {
                    var response = await _statisticService.GetMostValuableCustomersAsync(customerCount, _startDate, _endDate);

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

        //   GET/api/statistic/income/{categoryId}?startDate= 2020-7-21&endDate= 2020-7-22
        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<decimal>> GetIncomeOfCategory
    (int categoryId, [FromQuery] string startDate = null, [FromQuery] string endDate = null)
        {
            try
            {
                DateTime _startDate;
                DateTime _endDate;
                var formats = new[] { "yyyy-M-d", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d" };
                var culture = CultureInfo.InvariantCulture;

                if (string.IsNullOrEmpty(startDate))
                {
#pragma warning disable S6562
                    _startDate = DateTime.SpecifyKind(new DateTime(1972, 1, 1), DateTimeKind.Utc);
#pragma warning restore S6562
                }
                else if (!DateTime.TryParseExact(startDate, formats, culture, DateTimeStyles.None, out _startDate))
                {
                    return BadRequest("Invalid start date format. Please use yyyy-MM-dd format.");
                }

                if (string.IsNullOrEmpty(endDate))
                {
                    _endDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                }
                else if (!DateTime.TryParseExact(endDate, formats, culture, DateTimeStyles.None, out _endDate))
                {
                    return BadRequest("Invalid end date format. Please use yyyy-MM-dd format.");
                }

                var response = await _statisticService.GetIncomeOfCategoryInPeriod(categoryId, _startDate, _endDate);

                return Ok(response);
            }
            catch { return BadRequest(); }
        }
    }
}
