using System.IO;

namespace DragonAsia.SalesMan.Repositories
{
    class IoHelper
    {
        public static string GetNewGuid(string path, string ext)
        {
            for (int i = 0; i < int.MaxValue; i++)
                if (!File.Exists(path + "/" + i + ext))
                    return i.ToString();

            return string.Empty;
        }
    }
}
