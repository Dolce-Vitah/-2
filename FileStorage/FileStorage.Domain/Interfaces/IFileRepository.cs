using FileStorage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<StoredFile>> GetAllByHashAsync(string hash);
        Task<StoredFile?> GetByIdAsync(Guid id);
        Task AddAsync(StoredFile file);
    }
}
