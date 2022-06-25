using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.interfaces;
using core.models;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{

    [ApiController]
    [Route("api/filesystem/[controller]")]
    public class FolderController : ControllerBase
    {
        readonly IFolderService _folderService;
        IFileService _fileService;

        public FolderController(IFolderService fsService, IFileService fileService)
        {
            _folderService = fsService;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<folder> GetById(Guid id)
        {
            return await _folderService.GetByIdAsync(id);
        }

        [HttpGet]
        [Route("{parentId}/children")]
        public async Task<IEnumerable<FsInfo>> GetChildren(Guid parentId)
        {
            var ret = new List<FsInfo>();
            var folder = await _folderService.GetByIdAsync(parentId);

            foreach (var item in folder?.Children)
            {
                var cfolder = await _folderService.GetByIdAsync(item.Id);
                if (cfolder != null)
                {
                    ret.Add(await _folderService.GetInfoAsync(cfolder.Id));
                    continue;
                }
                var cfile = await _fileService.GetByIdAsync(item.Id);
                if (cfile != null)
                {
                    ret.Add(await _fileService.GetInfoAsync(cfile.Id));
                }
            }
            return ret;
        }

        [HttpGet]
        [Route("{id}/info")]
        public async Task<FsInfo> GetInfoById(Guid id)
        {
            return await _folderService.GetInfoAsync(id);
        }

        [HttpPost]
        public async Task<FsInfo> Create([FromBody] folder newObject)
        {
            if (await _folderService.Save(newObject))
            {
                return await _folderService.GetInfoAsync(newObject.Id);
            }
            return null;
        }

        [HttpPatch]
        public async Task<bool> Update(folder updatedObj)
        {
            return await _folderService.Save(updatedObj);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _folderService.Delete(id);
        }

        [HttpPost]
        [Route("{id}/copy")]
        public async Task<bool> Copy(Guid id)
        {
            return await _folderService.DeepCopyAsync(id);
        }
    }
}