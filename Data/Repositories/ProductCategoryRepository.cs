using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly TradeMarketDbContext _context;

        public ProductCategoryRepository(TradeMarketDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductCategory entity)
        {
            await _context.ProductCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(ProductCategory entity)
        {
            _context.ProductCategories.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _context.ProductCategories.FindAsync(id);
            if (entity != null)
            {
                _context.ProductCategories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.FindAsync(id);
        }

        public void Update(ProductCategory entity)
        {
            _context.ProductCategories.Update(entity);
            _context.SaveChanges();
        }
    }
}
