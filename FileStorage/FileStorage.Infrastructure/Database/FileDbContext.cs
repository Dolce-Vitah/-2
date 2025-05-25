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
    public class FileDbContext: DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options)
            : base(options)
        {
        }
        public DbSet<StoredFile> Files { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoredFile>()
                .HasKey(f => f.ID);
            
            modelBuilder.Entity<StoredFile>()
                .HasIndex(f => f.Hash)
                .IsUnique(false);
        }
    }
}
