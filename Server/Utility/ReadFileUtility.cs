namespace Server.Utility
{
    public static class ReadFileUtility
    {
        public static byte[] ReadFileHead(IFormFile file)
        {
            using var fs = new BinaryReader(file.OpenReadStream());
            var bytes = new byte[20];
            fs.Read(bytes, 0, 20);
            return bytes;
        }
    }
}
