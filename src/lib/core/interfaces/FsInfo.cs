using System;
using System.IO;

namespace core.interfaces
{
    public class FsInfo
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }

        public bool HasChildren { get; set; }
    }
}