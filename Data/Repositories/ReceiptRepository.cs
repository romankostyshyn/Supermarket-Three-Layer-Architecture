﻿using Data.Data;
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
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly TradeMarketDbContext _context;

        public ReceiptRepository(TradeMarketDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Receipt entity)
        {
            await _context.Receipts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(Receipt entity)
        {
            _context.Receipts.Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _context.Receipts.FindAsync(id);
            if (entity != null)
            {
                _context.Receipts.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await _context.Receipts.ToListAsync();
        }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            return await _context.Receipts
                .Include(r => r.Customer)
                    .ThenInclude(c => c.Person)
                .Include(r => r.ReceiptDetails)
                    .ThenInclude(rd => rd.Product)
                        .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Receipt> GetByIdAsync(int id)
        {
            return await _context.Receipts.FindAsync(id);
        }

        public async Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Receipts
                .Include(r => r.Customer)
                    .ThenInclude(c => c.Person)
                .Include(r => r.ReceiptDetails)
                    .ThenInclude(rd => rd.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public void Update(Receipt entity)
        {
            _context.Receipts.Update(entity);
            _context.SaveChanges();
        }
    }
}
