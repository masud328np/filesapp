using System;

namespace web.Controllers
{
    public class FolderMetaInfo
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public Guid Id { get; set; }
        public bool HasChildren { get; set; }

        public string Type { get; set; }
    }
}