using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {
            return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductModel>>(
                    (await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync())
                        .Where(el => el.CustomerId == customerId)
                        .SelectMany(el => el.ReceiptDetails)
                        .GroupBy(el => el.Product)
                        .OrderByDescending(el => el.Sum(x => x.Quantity))
                        .Select(el => el.Key)
                        .Take(productCount)
             );
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            return (await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync())
                 .Where(el => el.OperationDate.CompareTo(startDate) > 0 && el.OperationDate.CompareTo(endDate) < 0)
                 .SelectMany(el => el.ReceiptDetails)
                 .Where(el => el.Product.ProductCategoryId == categoryId)
                 .Sum(el => el.Quantity * el.DiscountUnitPrice);
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductModel>>(
                (await _unitOfWork.ReceiptDetailRepository.GetAllWithDetailsAsync())
                .GroupBy(el => el.Product)
                .OrderByDescending(el => el.Sum(x => x.Quantity))
                .Select(el => el.Key)
                .Take(productCount));
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            var receipt = (await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync())
                .Where(el => el.OperationDate.CompareTo(startDate) > 0 && el.OperationDate.CompareTo(endDate) < 0);


            IEnumerable<CustomerActivityModel> customerActivityModels = null;


            if (receipt.First().Customer.Person == null)
            {
                customerActivityModels = receipt.Select(el => new CustomerActivityModel()
                {
                    CustomerId = el.CustomerId,
                    CustomerName = (_unitOfWork.PersonRepository.GetByIdAsync(el.CustomerId).Result.Name)
                                     + " " +
                                     (_unitOfWork.PersonRepository.GetByIdAsync(el.CustomerId).Result.Surname),
                    ReceiptSum = el.ReceiptDetails.Sum(x => x.Quantity * x.DiscountUnitPrice)
                });
            }
            else
            {
                customerActivityModels = receipt.Select(el => new CustomerActivityModel()
                {
                    CustomerId = el.CustomerId,
                    CustomerName = el.Customer.Person.Name + " " + el.Customer.Person.Surname,
                    ReceiptSum = el.ReceiptDetails.Sum(x => x.Quantity * x.DiscountUnitPrice)
                });
            }


            return customerActivityModels.GroupBy(el => el.CustomerId)
                .Select(el => new CustomerActivityModel()
                {
                    CustomerId = el.First().CustomerId,
                    CustomerName = el.First().CustomerName,
                    ReceiptSum = el.Sum(x => x.ReceiptSum)
                })
                .OrderByDescending(el => el.ReceiptSum)
                .Distinct()
                .Take(customerCount);
        }
    }
}
