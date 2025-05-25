using FileAnalysis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.Domain.Interfaces
{
    public interface IAnalysisRepository
    {
        Task<AnalysisResult?> GetByIdAsync(Guid fileId);
        Task AddAsync(AnalysisResult result);
    }
}
