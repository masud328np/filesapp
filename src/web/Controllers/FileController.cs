using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using core.interfaces;
using core.models;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    [ApiController]
    [Route("api/filesystem/[controller]")]
    public class FileController : ControllerBase
    {
        readonly IFileService _fService;
       // readonly IFolderService _folderService;

        public FileController(IFileService fsService,
        IFolderService folderService
        )
        {
            _fService = fsService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<file> GetById(Guid id)
        {
            return await _fService.GetByIdAsync(id);
        }

        [HttpGet]
        [Route("{id}/info")]
        public async Task<FsInfo> GetInfoById(Guid id)
        {
            return await _fService.GetInfoAsync(id);
        }

        [HttpPatch]
        public async Task<bool> Update(file updatedObj)
        {
            return await _fService.UpdateAsync(updatedObj);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _fService.DeleteAsync(id);
        }

        [HttpPost]
        [Route("{id}/upload")]
        public async Task<bool> Upload([FromRoute] Guid id)
        {
            if (!Request.Form.Files.Any()) return false;
            var formFile = Request.Form.Files[0];
            var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');

            if (!await _fService.OverWriteAsync(
                 id, fileName, formFile.Length,
                 async x => await formFile.CopyToAsync(x)
                 ))
            {
                return await _fService.PersistAsync(id, fileName, formFile.Length,
      x => formFile.CopyTo(x)
      );
            }

            return true;
        }

        [HttpGet]
        [Route("{id}/download")]
        public async Task<FileResult> Download(Guid id)
        {
            var fileInfo = await _fService.GetInfoAsync(id);
            byte[] bytes = await _fService.ReadBytesAsync(id);
            return File(bytes, "application/octet-stream", fileInfo.Name);
        }

        [HttpPost]
        [Route("{id}/copy")]
        public async Task<bool> Copy(Guid id)
        {
            return await _fService.DeepCopyAsync(id);
        }

    }
}