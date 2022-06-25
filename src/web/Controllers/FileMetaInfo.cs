namespace web.Controllers
{
    public class FileMetaInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public long Size { get; set; }

        public string Type { get; set; }

        public bool HasChildren { get; set; }
    }
}