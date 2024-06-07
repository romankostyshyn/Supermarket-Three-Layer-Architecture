using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(CustomerModel model)
        {
            if (model == null)
            {
                throw new MarketException("Customer model cannot be null.");
            }

            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Surname))
            {
                throw new MarketException("Customer name cannot be empty.");
            }

            DateTime birthDateStart = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

            if (model.BirthDate < birthDateStart || model.BirthDate > today)
            {
                throw new MarketException("Invalid birth date.");
            }

            if (model.DiscountValue < 0)
            {
                throw new MarketException(nameof(model.DiscountValue));
            }

            await _unitOfWork.CustomerRepository.AddAsync(_mapper.Map<Customer>(model));
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _unitOfWork.CustomerRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<CustomerModel>>(entities);
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<CustomerModel>(entity);
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            var filteredCustomers = customers
                .Where(c => c.Receipts.Any(r => r.ReceiptDetails.Any(rd => rd.ProductId == productId)));

            return _mapper.Map<IEnumerable<CustomerModel>>(filteredCustomers);
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            if (model == null)
            {
                throw new MarketException("Customer model cannot be null.");
            }

            if (model.BirthDate.Year < 1900 || model.BirthDate > DateTime.Today)
            {
                throw new MarketException("Birth date is not correct.");
            }

            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Surname))
            {
                throw new MarketException("Name or surname cannot be empty.");
            }

            if (model.DiscountValue < 0)
            {
                throw new MarketException(nameof(model.DiscountValue));
            }

            var customer = _mapper.Map<CustomerModel, Customer>(model);
            var person = customer.Person;
            person.Id = customer.Id;
            customer.PersonId = customer.Id;
            _unitOfWork.PersonRepository.Update(person);


            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();
        }
    }
}
