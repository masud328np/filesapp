using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using core.interfaces;
using core.models;

namespace web.Controllers
{
    public interface IFolderService
    {
        Task<bool> Save(folder updatedObj);
        Task<bool> Delete(Guid id);
        Task<folder> GetByIdAsync(Guid id);
        Task<FsInfo> GetInfoAsync(Guid id);
        Task<bool> DeepCopyAsync(Guid id);
    }

    public class FolderService : IFolderService
    {
        private IContentProvider _contentProvider;
        private IFileService _fileService;

        public FolderService(
            IContentProvider contentProvider
            , IFileService fileService
            )
        {
            _contentProvider = contentProvider;
            _fileService = fileService;

        }
        public async Task<bool> Delete(Guid id)
        {
            var folder = await _contentProvider.LookupAsync<folder>(id);
            if (folder != null)
            {
                var children = folder.Children.Select(x => x.Id).ToList();
                foreach (var cId in children)
                {
                    await _contentProvider.RemoveAsync(cId);
                }

                return await _contentProvider.RemoveAsync(id);
            }
            return false;
        }

        public async Task<folder> GetByIdAsync(Guid id)
        {
            return await _contentProvider.LookupAsync<folder>(id);
        }

        public async Task<FsInfo> GetInfoAsync(Guid id)
        {
            var folder = await _contentProvider.LookupAsync<folder>(id);
            if (folder == null) return null;

            return new FsInfo()
            {
                Id = folder.Id,
                Name = folder.Name,
                Type = folder.Type.ToString(),
                HasChildren = folder.Children.Any(),
                Size = await Calculate(folder)
            };
        }

        private async Task<long> Calculate(folder folder)
        {
            long size = 0;
            foreach (var item in folder.Children)
            {
                var fsObj = await _contentProvider.LookupAsync<IFsEntity>(item.Id);
                if (fsObj != null) size += fsObj.Size;
            }

            return size;
        }

        public async Task<bool> Save(folder obj)
        {
            var folder = await _contentProvider.LookupAsync<folder>(obj.Id);
            if (folder == null)
            {
                obj.Id = Guid.NewGuid();
                await _contentProvider.CreateAsync(obj);
                if (obj.ParentId.HasValue)
                {
                    var parentFolder = await _contentProvider.LookupAsync<folder>(obj.ParentId.Value);
                    parentFolder?.Children.Add(obj);
                }
                return true;
            }
            if (folder == null) return false;
            folder.Name = obj.Name;

            return true;

        }

        public async Task<bool> DeepCopyAsync(Guid id)
        {
            var oldFolder = await _contentProvider.LookupAsync<folder>(id);
            var newFolder = new folder()
            {
                Name = oldFolder + " copy",
                ParentId = oldFolder.ParentId.Value,
            };
            await CopyFolders(oldFolder, newFolder);
            return newFolder.Id != default(Guid);
        }



        private async Task CopyFiles(file srcfile)
        {
            await _fileService.DeepCopyAsync(srcfile.Id, "");
        }

        private async Task CopyFolders(folder oldObj, folder newObject)
        {
            if (await Save(newObject))
            {
                foreach (var item in oldObj.Children)
                {
                    if (item.Type == fileSystemType.file)
                    {
                        item.ParentId = newObject.Id;
                        await CopyFiles(item as file);
                        continue;
                    }
                    var newFolder = new folder()
                    {
                        Name = item.Name,
                        ParentId = newObject.Id
                    };
                    newObject.Children.Add(newFolder);
                    await CopyFolders(item as folder, newFolder);
                }

            }

        }
    }
}