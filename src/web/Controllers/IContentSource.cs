using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.interfaces;

namespace web.Controllers
{
    public interface IContentSource
    {
        Task<bool> PersistAsync<T>(T newObject) where T : class, IFsEntity;
        Task<IEnumerable<IFsEntity>> GetAllAsync();
        Task<T> LookupAsync<T>(Guid id) where T : class, IFsEntity;
        Task<bool> DeleteAsync(Guid id);
    }

    public class MemoryContentProvider : IContentSource
    {
        private IDictionary<Guid, IFsEntity> _content;

        public MemoryContentProvider(IDictionary<Guid, IFsEntity> content)
        {
            _content = content;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await Task.Run(() => _content.Remove(id));
        }

        public async Task<IEnumerable<IFsEntity>> GetAllAsync()
        {
            return await Task.Run(() => _content.Select(x => x.Value));
        }

        public async Task<T> LookupAsync<T>(Guid id) where T : class, IFsEntity
        {
            if (_content.TryGetValue(id, out IFsEntity value))
            {
                return await Task.Run(() => value as T);
            }
            return default(T);

        }

        public async Task<bool> PersistAsync<T>(T newObject) where T : class, IFsEntity
        {
            if (_content.TryAdd(newObject.Id, newObject))
            {
                return await Task.Run(() => true);
            }
            return false;
        }
    }
}