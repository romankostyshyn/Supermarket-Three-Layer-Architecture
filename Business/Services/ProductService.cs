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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(ProductModel model)
        {
            if (model == null)
            {
                throw new MarketException("Product model cannot be null.");
            }

            if (string.IsNullOrEmpty(model.ProductName))
            {
                throw new MarketException("Customer name cannot be empty.");
            }

            if (model.Price < 0)
            {
                throw new MarketException("Product price cannot be negative.");
            }

            var productEntity = _mapper.Map<ProductModel, Product>(model);
            productEntity.Category = _unitOfWork.ProductCategoryRepository.GetByIdAsync(productEntity.ProductCategoryId).Result;

            await _unitOfWork.ProductRepository.AddAsync(productEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            if (categoryModel == null)
            {
                throw new MarketException("Product category model cannot be null.");
            }

            if (string.IsNullOrEmpty(categoryModel.CategoryName))
            {
                throw new MarketException("Category name cannot be empty.");
            }

            var categoryEntity = _mapper.Map<ProductCategory>(categoryModel);
            await _unitOfWork.ProductCategoryRepository.AddAsync(categoryEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _unitOfWork.ProductRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var productEntities = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ProductModel>>(productEntities);
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var categoryEntities = await _unitOfWork.ProductCategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductCategoryModel>>(categoryEntities);
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            var productEntities = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();
            var filteredProducts = productEntities.Where(p =>
                (!filterSearch.CategoryId.HasValue || p.ProductCategoryId == filterSearch.CategoryId) &&
                (!filterSearch.MinPrice.HasValue || p.Price >= filterSearch.MinPrice) &&
                (!filterSearch.MaxPrice.HasValue || p.Price <= filterSearch.MaxPrice)
            );

            return _mapper.Map<IEnumerable<ProductModel>>(filteredProducts);
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            var productEntity = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);
            if (productEntity == null)
            {
                throw new MarketException("Product not found.");
            }

            return _mapper.Map<ProductModel>(productEntity);
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            await _unitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ProductModel model)
        {
            if (model == null || model.ProductName == null || string.IsNullOrEmpty(model.ProductName))
            {
                throw new MarketException("Product cannot be null or empty.");
            }
            if (model.Price < 0)
            {
                throw new MarketException("Price must be higher than 0.");
            }

            var product = _mapper.Map<ProductModel, Product>(model);
            var category = product.Category;
            category.Id = model.ProductCategoryId;
            category.CategoryName = model.CategoryName;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            if (categoryModel == null || categoryModel.CategoryName == null || string.IsNullOrEmpty(categoryModel.CategoryName))
            {
                throw new MarketException("Category cannot be null or empty.");
            }

            _unitOfWork.ProductCategoryRepository.Update(_mapper.Map<ProductCategoryModel, ProductCategory>(categoryModel));
            await _unitOfWork.SaveAsync();
        }
    }

}
