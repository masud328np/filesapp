using System;
using System.IO;
using System.Threading.Tasks;

namespace web.Controllers
{
    public interface IStorageProvider
    {
        Task RemoveAsync(string path);
        Task<byte[]> ReadAsync(string path);
        Task<bool> WriteAsync(string path, Action<Stream> write);
    }

    public class FileStorageProvider : IStorageProvider
    {
        readonly string _basePath;
        public FileStorageProvider(string basePath)
        {
            _basePath = basePath;
            Directory.CreateDirectory(_basePath);
        }
        public async Task<byte[]> ReadAsync(string path)
        {
            var filePath = Path.Combine(_basePath, path);
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }
            return await System.IO.File.ReadAllBytesAsync(filePath);
        }

        public async Task RemoveAsync(string path)
        {
            await Task.Run(() => System.IO.File.Delete(Path.Combine(_basePath, path)));
        }

        public async Task<bool> WriteAsync(string path, Action<Stream> write)
        {
            Directory.CreateDirectory(_basePath);
            await Task.Run(() =>
            {
                using (var stream = System.IO.File.Create(Path.Combine(_basePath, path)))
                {
                    write(stream);
                }
            });
            return true;
        }


    }
}