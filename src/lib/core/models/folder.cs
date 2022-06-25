
using System;
using System.Collections.Generic;
using System.Linq;
using core.interfaces;

namespace core.models
{
    public class folder : IFsEntity
    {
        public folder()
        {
            Children = new List<IFsEntity>();
        }
        public IList<IFsEntity> Children { get; set; }
        public fileSystemType Type => fileSystemType.folder;
        public string Name { get; set; }
        public long Size
        {
            get
            {
                return Children.Sum(x => x.Size);
            }
            set { }
        }

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
    }
}