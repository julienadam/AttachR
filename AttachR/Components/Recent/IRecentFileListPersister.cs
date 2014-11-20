using System.Collections.Generic;

namespace AttachR.Components.Recent
{
    public interface IRecentFileListPersister
    {
        List<string> RecentFiles();
        void InsertFile(string filepath);
        void RemoveFile(string filepath);
    }
}