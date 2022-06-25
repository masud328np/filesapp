using System;
using System.IO;
using System.Threading.Tasks;
using core.interfaces;
using core.models;

namespace web.Controllers
{
    public interface IFileService
    {
        Task<file> GetByIdAsync(Guid id);
        Task<bool> PersistAsync(Guid parentId, string name, long size, Action<Stream> write);
        Task<bool> DeleteAsync(Guid id);
        Task<byte[]> ReadBytesAsync(Guid id);
        Task<FsInfo> GetInfoAsync(Guid id);
        Task<bool> CreateAsync(file updatedObj);

        Task<bool> UpdateAsync(file updatedObj);
        Task<bool> OverWriteAsync(Guid id, string fileName, long size, Action<Stream> p);
        Task<bool> DeepCopyAsync(Guid id, string suffix = " copy");
    }

    public class FileService : IFileService
    {
        private IContentProvider _contentProvider;

        public FileService(
            IContentProvider contentProvider
            )
        {
            _contentProvider = contentProvider;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _contentProvider.RemoveAsync(id);
        }

        public async Task<file> GetByIdAsync(Guid id)
        {
            return await _contentProvider.LookupAsync<file>(id);
        }

        public async Task<FsInfo> GetInfoAsync(Guid id)
        {
            var file = await GetByIdAsync(id);
            if (file != null)
            {
                return new FsInfo()
                {
                    Id = file.Id,
                    Name = file.Name,
                    Type = file.Type.ToString(),
                    Size = file.Size,
                    HasChildren = false
                };
            }

            return null;
        }

        public async Task<bool> PersistAsync(Guid parentId, string name, long size, Action<Stream> write)
        {
            if (size < 0) return false;

            var parentFolder = await _contentProvider.LookupAsync<folder>(parentId);
            if (parentFolder == null) return false;

            var newFile = new file()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Size = size,
                ParentId = parentFolder.Id
            };

            if (await _contentProvider.WriteAsync(newFile.Id.ToString(), write))
            {
                if (await CreateAsync(newFile))
                {
                    parentFolder.Children.Add(newFile);
                    return true;
                }
            }

            return false;

        }



        public async Task<bool> CreateAsync(file obj)
        {
            if (obj.Id == default(Guid)) return false;
            return await _contentProvider.CreateAsync(obj);
        }

        public async Task<bool> UpdateAsync(file obj)
        {
            var info = await GetByIdAsync(obj.Id);
            if (info == null) return false;
            info.Name = obj.Name;
            return true;
        }

        public async Task<byte[]> ReadBytesAsync(Guid id)
        {
            var file = await _contentProvider.LookupAsync<file>(id);
            return await _contentProvider.ReadAsync(file.Id.ToString());
        }

        public async Task<bool> OverWriteAsync(Guid id, string fileName, long size, Action<Stream> p)
        {
            if (size <= 0) return false;
            var file = await GetByIdAsync(id);
            if (file == null) return false;

            if (!await _contentProvider.WriteAsync(file.Id.ToString(), p))
            {
                return false;
            }

            file.Name = fileName;
            file.Size = size;
            return true;
        }

        public async Task<bool> DeepCopyAsync(Guid id, string suffix)
        {
            var file = await _contentProvider.LookupAsync<file>(id);
            if (file != null)
            {
                var content = await ReadBytesAsync(id);
                return await PersistAsync(file.ParentId.Value
                , file.Name + suffix, content.Length, (x) =>
                {
                    using (var writer = new BinaryWriter(x))
                    {
                        writer.Write(content);
                    }
                });
            }

            return false;
        }
    }
}