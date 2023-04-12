using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public class FileManager :
        IFileManager
    {
        public async Task<Stream> ReadFromFile(string filepath)
        {
            var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            return stream;
        }

        public Task WriteToFile(Stream stream, string? filepath = null)
        {
            if(filepath == null)
            {
                return Task.CompletedTask;
            }
            using(Stream filestream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(filestream);
            }
            return Task.CompletedTask;
        }
    }
}
