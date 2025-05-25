using FileAnalysis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.UseCases.Interfaces
{
    public interface IStatisticsService
    {
        Task<AnalysisResult> AnalyzeFile(Guid id, Stream content);
    }
}
