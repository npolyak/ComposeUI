using System.IO;

namespace Sebastion.Core
{
    public class FileInfoVm
    {
        private FileInfo _fileInfo = null; 

        public string FullPath => _fileInfo?.FullName;

        public string FileName => _fileInfo?.Name;

        public string FolderName => _fileInfo?.DirectoryName;

        public FileInfoVm(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                _fileInfo = new FileInfo(fullPath);
            }
        }

        public override string ToString()
        {
            return FullPath;
        }
    }
}
