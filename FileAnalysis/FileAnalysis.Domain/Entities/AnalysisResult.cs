using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.Domain.Entities
{
    public class AnalysisResult
    {
        public Guid ID { get; private set; }
        public int ParagraphCount { get; private set; }
        public int WordCount { get; private set; }
        public int CharacterCount { get; private set; }

        public AnalysisResult() { }

        public AnalysisResult(Guid id, int paragraphCount, int wordCount, int characterCount)
        {
            ID = id;
            ParagraphCount = paragraphCount;
            WordCount = wordCount;
            CharacterCount = characterCount;
        }
    }
}
