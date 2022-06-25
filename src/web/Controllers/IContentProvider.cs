using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using core.interfaces;
using core.models;

namespace web.Controllers
{
    public interface IContentProvider
    {
        Task<IEnumerable<IFsEntity>> GetAllAsync();
        Task<bool> RemoveAsync(Guid id);
        Task<T> LookupAsync<T>(Guid id) where T : class, IFsEntity;
        Task<bool> CreateAsync<T>(T newObject) where T : class, IFsEntity;
        Task<bool> WriteAsync(string path, Action<Stream> write);
        Task<byte[]> ReadAsync(string path);
    }

    public class FileSystemContentProvider : IContentProvider
    {
        private IStorageProvider _storage;
        private IContentSource _dataSrc;

        public FileSystemContentProvider(IStorageProvider storage, IContentSource dataSrc)
        {
            _storage = storage;
            _dataSrc = dataSrc;
        }
        public async Task<bool> CreateAsync<T>(T newObject)
        where T : class, IFsEntity
        {
            return await _dataSrc.PersistAsync(newObject);
        }

        public async Task<IEnumerable<IFsEntity>> GetAllAsync()
        {
            return await _dataSrc.GetAllAsync();
        }
        public async Task<T> LookupAsync<T>(Guid id) where T : class, IFsEntity
        {
            return await _dataSrc.LookupAsync<T>(id);
        }

        public async Task<byte[]> ReadAsync(string path)
        {
            return await _storage.ReadAsync(path);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var obj = await _dataSrc.LookupAsync<IFsEntity>(id);
            await _storage.RemoveAsync(obj.Id.ToString());
            if (await _dataSrc.DeleteAsync(id))
            {
                var parent = await _dataSrc.LookupAsync<IFsEntity>(obj.ParentId.Value);
                parent.Children.Remove(parent.Children.FirstOrDefault(x => x.Id == id));
                return true;
            }
            return false;
        }

        public async Task<bool> WriteAsync(string path, Action<Stream> write)
        {
            return await _storage.WriteAsync(path, write);
        }
    }
}