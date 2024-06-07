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
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddAsync(ReceiptModel model)
        {
            var receiptEntity = _mapper.Map<Receipt>(model);
            await _unitOfWork.ReceiptRepository.AddAsync(receiptEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt == null)
            {
                throw new MarketException("Receipt not found.");
            }

            if (receipt.ReceiptDetails.Any(el => el.ProductId == productId))
            {
                var res = receipt.ReceiptDetails.First(el => el.ProductId == productId);
                res.Quantity += quantity;

                _unitOfWork.ReceiptDetailRepository.Update(res);
            }
            else
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new MarketException("Product not found.");
                }

                var discount = receipt.Customer.DiscountValue;
                var discountUnitPrice = product.Price - (product.Price * discount / 100);

                var receiptDetail = new ReceiptDetail
                {
                    ProductId = productId,
                    ReceiptId = receiptId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountUnitPrice = discountUnitPrice
                };

                await _unitOfWork.ReceiptDetailRepository.AddAsync(receiptDetail);
            }
            await _unitOfWork.SaveAsync();
        }

        public async Task CheckOutAsync(int receiptId)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdAsync(receiptId);
            if (receipt == null)
            {
                throw new MarketException("Receipt does not exist.");
            }

            receipt.IsCheckedOut = true;

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(modelId);
            if (receipt == null)
            {
                throw new MarketException("Receipt does not exist.");
            }

            foreach (var receiptDetail in receipt.ReceiptDetails.ToList())
            {
                _unitOfWork.ReceiptDetailRepository.Delete(receiptDetail);
            }

            await _unitOfWork.ReceiptRepository.DeleteByIdAsync(modelId);

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            var receiptEntities = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReceiptModel>>(receiptEntities);
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            var receiptEntity = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ReceiptModel>(receiptEntity);
        }

        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt == null)
            {
                throw new MarketException("Receipt does not exist.");
            }

            return _mapper.Map<IEnumerable<ReceiptDetailModel>>(receipt.ReceiptDetails);
        }

        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var filteredReceipts = receipts.Where(r => r.OperationDate >= startDate && r.OperationDate < endDate.AddDays(1));

            return _mapper.Map<IEnumerable<ReceiptModel>>(filteredReceipts);
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt == null)
            {
                throw new MarketException("Receipt does not exist.");
            }

            var receiptDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

            if (receiptDetail == null)
            {
                throw new MarketException("Product not found in receipt.");
            }

            receiptDetail.Quantity = Math.Max(receiptDetail.Quantity - quantity, 0);

            if (receiptDetail.Quantity == 0)
            {
                receipt.ReceiptDetails.Remove(receiptDetail);
                _unitOfWork.ReceiptDetailRepository.Delete(receiptDetail);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            decimal sum = 0;
            foreach (var item in receipt.ReceiptDetails)
                sum += item.DiscountUnitPrice * item.Quantity;

            return sum;
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            _unitOfWork.ReceiptRepository.Update(_mapper.Map<ReceiptModel, Receipt>(model));
            await _unitOfWork.SaveAsync();
        }
    }
}
