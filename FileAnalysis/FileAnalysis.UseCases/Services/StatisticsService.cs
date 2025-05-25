using FileAnalysis.Domain.Entities;
using FileAnalysis.Domain.Interfaces;
using FileAnalysis.UseCases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.UseCases.Services
{
    internal class StatisticsService: IStatisticsService
    {
        private readonly IAnalysisRepository _analysisRepository;

        public StatisticsService(IAnalysisRepository analysisRepository)
        {
            _analysisRepository = analysisRepository;
        }

        public async Task<AnalysisResult> AnalyzeFile(Guid id, Stream content)
        {                       
            using var reader = new StreamReader(content, leaveOpen: true);
            content.Position = 0;
            string text = reader.ReadToEnd();

            var paragraphCount = text
                .Split([ "\r\n\r\n", "\n\n", "\r\n", "\n", "\r" ], StringSplitOptions.None)
                .Count(line => !string.IsNullOrWhiteSpace(line));

            var wordCount = text
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var characterCount = text.Length;

            var result = new AnalysisResult(
                id,
                paragraphCount,
                wordCount,
                characterCount
             );

            await _analysisRepository.AddAsync(result);
            return result;
        }
    }
    
}
