using System;
using System.IO;

namespace yatl.Utilities
{
    sealed class FileRefreshInfo
    {
        private readonly string path;
        private readonly string fileName;
        private DateTime lastModified;

        public string Path { get { return this.path; } }

        public string FileName { get { return this.fileName; } }

        public FileRefreshInfo(string path)
        {
            this.path = path;
            this.lastModified = File.GetLastWriteTime(path);

            this.fileName = System.IO.Path.GetFileName(path);
        }

        public bool WasModified(bool resetModified = true)
        {
            var modified = File.GetLastWriteTime(this.path);
            if (modified == this.lastModified)
                return false;
            if (resetModified)
                this.lastModified = modified;
            return true;
        }
    }
}
