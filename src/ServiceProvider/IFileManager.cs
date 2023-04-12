using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public interface IFileManager
    {
        Task WriteToFile(Stream stream, string? filepath = null);

        Task<Stream> ReadFromFile(string filepath);
    }
}
