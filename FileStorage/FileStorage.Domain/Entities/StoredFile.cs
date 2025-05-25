using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Domain.Entities
{
    public class StoredFile
    {
        public Guid ID { get; private set; }
        public string FileName { get; private set; } = null!;
        public string Hash { get; private set; } = null!;
        public string Location { get; private set; } = null!;

        public StoredFile() { }

        public StoredFile(Guid id, string fileName, string hash, string location)
        {
            ID = id;
            FileName = fileName;
            Hash = hash;
            Location = location;
        }
    }
}
