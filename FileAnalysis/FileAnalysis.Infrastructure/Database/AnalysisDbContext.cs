using FileAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysis.Infrastructure.Database
{
    public class AnalysisDbContext: DbContext
    {
        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options)
            : base(options)
        {
        }
        public DbSet<AnalysisResult> AnalysisResults { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalysisResult>()
                .HasKey(a => a.ID);

            modelBuilder.Entity<AnalysisResult>()
                .HasIndex(a => a.ID)
                .IsUnique();
        }
    }
}
