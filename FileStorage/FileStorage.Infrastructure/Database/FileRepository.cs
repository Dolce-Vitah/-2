using FileStorage.Domain.Entities;
using FileStorage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.Database
{
    public class FileRepository: IFileRepository
    {
        private readonly FileDbContext _context;

        public FileRepository(FileDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StoredFile>> GetAllByHashAsync(string hash)
        {
            return await _context.Files
                .Where(f => f.Hash == hash)
                .ToListAsync();
        }

        public async Task<StoredFile?> GetByIdAsync(Guid id)
        {
            return await _context.Files
                .FirstOrDefaultAsync(f => f.ID == id);
        }

        public async Task AddAsync(StoredFile file)
        {
            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();
        }
    }
}
