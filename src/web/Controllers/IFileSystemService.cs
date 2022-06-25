using System;
using System.Threading.Tasks;

namespace web.Controllers
{
    public interface IFileSystemService
    {
        Task<T> GetById<T>(Guid id);
        Task<T> GetInfo<T>(Guid id);
    }
}