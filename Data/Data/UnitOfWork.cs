using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TradeMarketDbContext _context;
        private ICustomerRepository _customerRepository;
        private IPersonRepository _personRepository;
        private IProductRepository _productRepository;
        private IProductCategoryRepository _productCategoryRepository;
        private IReceiptRepository _receiptRepository;
        private IReceiptDetailRepository _receiptDetailRepository;

        public UnitOfWork(TradeMarketDbContext context)
        {
            _context = context;

            _customerRepository = new CustomerRepository(_context);
            _personRepository = new PersonRepository(_context);
            _productRepository = new ProductRepository(_context);
            _productCategoryRepository = new ProductCategoryRepository(_context);
            _receiptRepository = new ReceiptRepository(_context);
            _receiptDetailRepository = new ReceiptDetailRepository(_context);
        }

        public ICustomerRepository CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                {
                    _customerRepository = new CustomerRepository(_context);
                }
                return _customerRepository;
            }
        }

        public IPersonRepository PersonRepository
        {
            get
            {
                if (_personRepository == null)
                {
                    _personRepository = new PersonRepository(_context);
                }
                return _personRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            }
        }

        public IProductCategoryRepository ProductCategoryRepository
        {
            get
            {
                if (_productCategoryRepository == null)
                {
                    _productCategoryRepository = new ProductCategoryRepository(_context);
                }
                return _productCategoryRepository;
            }
        }

        public IReceiptRepository ReceiptRepository
        {
            get
            {
                if (_receiptRepository == null)
                {
                    _receiptRepository = new ReceiptRepository(_context);
                }
                return _receiptRepository;
            }
        }

        public IReceiptDetailRepository ReceiptDetailRepository
        {
            get
            {
                if (_receiptDetailRepository == null)
                {
                    _receiptDetailRepository = new ReceiptDetailRepository(_context);
                }
                return _receiptDetailRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
