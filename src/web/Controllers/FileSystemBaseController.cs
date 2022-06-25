using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    public class FileSystemBaseController : ControllerBase
    {
        readonly IFileSystemService _fsService;

        public FileSystemBaseController(IFileSystemService fsService
        )
        {
            _fsService = fsService;
        }

        public virtual async Task<T> GetById<T>(Guid id)
        {
            return await _fsService.GetById<T>(id);
        }

        public virtual async Task<T> GetInfoById<T>(Guid id)
        {
            return await _fsService.GetInfo<T>(id);
        }
    }

}