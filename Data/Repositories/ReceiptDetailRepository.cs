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
    public class ReceiptDetailRepository : IReceiptDetailRepository
    {
        private readonly TradeMarketDbContext _context;

        public ReceiptDetailRepository(TradeMarketDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReceiptDetail entity)
        {
            await _context.ReceiptsDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(ReceiptDetail entity)
        {
            _context.ReceiptsDetails.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _context.ReceiptsDetails.FirstOrDefaultAsync(rd => rd.ReceiptId == id);
            if (entity != null)
            {
                _context.ReceiptsDetails.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllAsync()
        {
            return await _context.ReceiptsDetails.ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync()
        {
            return await _context.ReceiptsDetails
                .Include(rd => rd.Receipt)
                    .ThenInclude(r => r.Customer)
                        .ThenInclude(c => c.Person)
                .Include(rd => rd.Product)
                    .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<ReceiptDetail> GetByIdAsync(int id)
        {
            return await _context.ReceiptsDetails.FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public void Update(ReceiptDetail entity)
        {
            _context.ReceiptsDetails.Update(entity);
            _context.SaveChanges();
        }
    }
}
