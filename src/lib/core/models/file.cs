using System;
using System.Collections.Generic;
using System.IO;
using core.interfaces;

namespace core.models
{
    public class file : IFsEntity
    {
        public fileSystemType Type => fileSystemType.file;

        public string Name { get; set; }
        public long Size { set; get; }

        public Guid Id { get; set; }
        public IList<IFsEntity> Children
        {
            get => new List<IFsEntity> { };
            set => throw new NotImplementedException();
        }
        public Guid? ParentId  { get; set; }
    }
}