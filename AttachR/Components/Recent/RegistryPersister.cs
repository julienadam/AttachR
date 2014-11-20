using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace AttachR.Components.Recent
{
    public class RegistryPersister : IRecentFileListPersister
    {
        private const int MaxNumberOfFiles = 9;
        private string RegistryKey { get; set; }

        public RegistryPersister()
        {
            RegistryKey =
                "Software\\" +
                RecentFileList.ApplicationAttributes.CompanyName + "\\" +
                RecentFileList.ApplicationAttributes.ProductName + "\\" +
                "RecentFileList";
        }

        public RegistryPersister(string key)
        {
            RegistryKey = key;
        }

        string Key(int i) { return i.ToString("00"); }

        public List<string> RecentFiles()
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey) ?? Registry.CurrentUser.CreateSubKey(RegistryKey);

            List<string> list = new List<string>(MaxNumberOfFiles);

            for (int i = 0; i < MaxNumberOfFiles; i++)
            {
                string filename = (string)k.GetValue(Key(i));

                if (String.IsNullOrEmpty(filename)) break;

                list.Add(filename);
            }

            return list;
        }

        public void InsertFile(string filepath)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) Registry.CurrentUser.CreateSubKey(RegistryKey);
            k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

            RemoveFile(filepath);

            for (int i = MaxNumberOfFiles - 2; i >= 0; i--)
            {
                string sThis = Key(i);
                string sNext = Key(i + 1);

                object oThis = k.GetValue(sThis);
                if (oThis == null) continue;

                k.SetValue(sNext, oThis);
            }

            k.SetValue(Key(0), filepath);
        }

        public void RemoveFile(string filepath)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
            if (k == null) return;

            for (int i = 0; i < MaxNumberOfFiles; i++)
            {
                again:
                string s = (string)k.GetValue(Key(i));
                if (s != null && s.Equals(filepath, StringComparison.CurrentCultureIgnoreCase))
                {
                    RemoveFile(i, MaxNumberOfFiles);
                    goto again;
                }
            }
        }

        void RemoveFile(int index, int max)
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
            if (k == null) return;

            k.DeleteValue(Key(index), false);

            for (int i = index; i < max - 1; i++)
            {
                string sThis = Key(i);
                string sNext = Key(i + 1);

                object oNext = k.GetValue(sNext);
                if (oNext == null) break;

                k.SetValue(sThis, oNext);
                k.DeleteValue(sNext);
            }
        }
    }
}