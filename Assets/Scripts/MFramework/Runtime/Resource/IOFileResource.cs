using System.IO;

namespace Wx.Runtime.Resource
{
    public class IOFileResource : IResourceBase
    {
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}