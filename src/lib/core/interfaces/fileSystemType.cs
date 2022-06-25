using System;
using System.Collections.Generic;

namespace core.interfaces
{
    public enum fileSystemType
    {
        file = 0,
        folder = 1
    }


    public interface iFileSystemActionProvider
    {
        void Delete(Guid Id);
        void Rename(Guid Id, string name);
        void Copy(Guid Id);
    }

    public interface iFileActionProvider : iFileSystemActionProvider
    {
        void Download(Guid Id);
    }

    public interface iFolderActionProvider : iFileSystemActionProvider
    {
        void CreateSubFolder(Guid Id, string name);
        void Upload(IFsEntity name);
    }

    public abstract class FileSystemActionProiderBase : iFileSystemActionProvider
    {
        public virtual void Copy(Guid Id) { }

        public virtual void Delete(Guid Id) { }

        public virtual void Rename(Guid Id, string name) { }
    }

    public class fileActionProvider : FileSystemActionProiderBase, iFileActionProvider
    {
        public void Download(Guid Id)
        {
            throw new NotImplementedException();
        }
    }

    public class folderActionProvider : FileSystemActionProiderBase, iFolderActionProvider
    {
        public virtual void CreateSubFolder(Guid Id, string name)
        {
            throw new NotImplementedException();
        }

        public virtual void Upload(IFsEntity name)
        {
            throw new NotImplementedException();
        }
    }

    public interface iFsMeta
    {
        string Name { get; set; }
        string Type { get; }
        long Size { get; }
    }

    public class FSMeta
    {
        string Name { get; set; }
        string Type { get; set; }
        long Size { get; set; }
    }
    public class FolderMeta : FSMeta
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }
    }

    public interface IFsEntity
    {
        Guid Id { get; set; }
        string Name { get; set; }
        fileSystemType Type { get; }
        long Size { get; set; }
        IList<IFsEntity> Children { get; set; }
        Guid? ParentId { get; set; }
    }
}