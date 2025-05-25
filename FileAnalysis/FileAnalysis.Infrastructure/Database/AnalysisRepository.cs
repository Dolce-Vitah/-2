using FileAnalysis.Domain.Entities;
using FileAnalysis.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.Infrastructure.Database
{
    public class AnalysisRepository: IAnalysisRepository
    {
        private readonly AnalysisDbContext _context;
        public AnalysisRepository(AnalysisDbContext context)
        {
            _context = context;
        }
        public async Task<AnalysisResult?> GetByIdAsync(Guid fileId)
        {
            return await _context.AnalysisResults.FirstOrDefaultAsync(r => r.ID == fileId);
        }
        public async Task AddAsync(AnalysisResult result)
        {
            await _context.AnalysisResults.AddAsync(result);
            await _context.SaveChangesAsync();
        }
    }
}
