using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace AttachR.Components.Recent
{
    public class RecentFileList : Separator
    {
        public IRecentFileListPersister Persister { get; set; }

        public int MaxNumberOfFiles { get; set; }
        public int MaxPathLength { get; set; }
        public MenuItem FileMenu { get; private set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
        /// Default = "_{0}:  {2}"
        /// </summary>
        public string MenuItemFormatOneToNine { get; set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
        /// Default = "{0}:  {2}"
        /// </summary>
        public string MenuItemFormatTenPlus { get; set; }

        public delegate string GetMenuItemTextDelegate(int index, string filepath);
        public GetMenuItemTextDelegate GetMenuItemTextHandler { get; set; }

        public event EventHandler<MenuClickEventArgs> MenuClick;

        Separator separator;
        List<RecentFile> recentFiles;

        public RecentFileList()
        {
            MaxPathLength = 50;
            MenuItemFormatOneToNine = "_{0}:  {2}";
            MenuItemFormatTenPlus = "{0}:  {2}";

            Loaded += (s, e) => HookFileMenu();
        }

        void HookFileMenu()
        {
            MenuItem parent = Parent as MenuItem;
            if (parent == null) throw new ApplicationException("Parent must be a MenuItem");

            if (Equals(FileMenu, parent)) return;

            if (FileMenu != null) FileMenu.SubmenuOpened -= _FileMenu_SubmenuOpened;

            FileMenu = parent;
            FileMenu.SubmenuOpened += _FileMenu_SubmenuOpened;
        }

        public List<string> RecentFiles { get { return Persister.RecentFiles(); } }
        public void RemoveFile(string filepath) { Persister.RemoveFile(filepath); }
        public void InsertFile(string filepath) { Persister.InsertFile(filepath); }

        void _FileMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            SetMenuItems();
        }

        void SetMenuItems()
        {
            RemoveMenuItems();

            LoadRecentFiles();

            InsertMenuItems();
        }

        void RemoveMenuItems()
        {
            if (separator != null)
            {
                FileMenu.Items.Remove(separator);
            }

            if (recentFiles != null)
            {
                foreach (RecentFile r in recentFiles)
                {
                    if (r.MenuItem != null)
                    {
                        FileMenu.Items.Remove(r.MenuItem);
                    }
                }
            }

            separator = null;
            recentFiles = null;
        }

        void InsertMenuItems()
        {
            if (recentFiles == null) return;
            if (recentFiles.Count == 0) return;

            int iMenuItem = FileMenu.Items.IndexOf(this);
            foreach (RecentFile r in recentFiles)
            {
                string header = GetMenuItemText(r.Number + 1, r.Filepath, r.DisplayPath);

                r.MenuItem = new MenuItem { Header = header };
                r.MenuItem.Click += MenuItem_Click;

                FileMenu.Items.Insert(++iMenuItem, r.MenuItem);
            }

            separator = new Separator();
            FileMenu.Items.Insert(++iMenuItem, separator);
        }

        string GetMenuItemText(int index, string filepath, string displaypath)
        {
            GetMenuItemTextDelegate delegateGetMenuItemText = GetMenuItemTextHandler;
            if (delegateGetMenuItemText != null) return delegateGetMenuItemText(index, filepath);

            string format = (index < 10 ? MenuItemFormatOneToNine : MenuItemFormatTenPlus);

            string shortPath = ShortenPathname(displaypath, MaxPathLength);

            return String.Format(format, index, filepath, shortPath);
        }

        // This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx

        /// <summary>
        /// Shortens a pathname for display purposes.
        /// </summary>
        /// <param labelName="pathname">The pathname to shorten.</param>
        /// <param labelName="maxLength">The maximum number of characters to be displayed.</param>
        /// <remarks>Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three elipses (...)
        /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
        /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.</para>
        /// <para>This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)</para>
        /// </remarks>
        /// <returns></returns>
        static public string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname.Length <= maxLength)
                return pathname;

            string root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
            {
                root += Path.DirectorySeparatorChar;
            }

            string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int filenameIndex = elements.GetLength(0) - 1;

            if (elements.GetLength(0) == 1) // pathname is just a root and filename
            {
                if (elements[0].Length > 5) // long enough to shorten
                {
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    else
                    {
                        return pathname.Substring(0, maxLength - 3) + "...";
                    }
                }
            }
            else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
            {
                root += "...\\";

                int len = elements[filenameIndex].Length;
                if (len < 6)
                    return root + elements[filenameIndex];

                if ((root.Length + 6) >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }
                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + "...\\" + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathname.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                        totalLength -= elements[--begin].Length - 1;

                    if (totalLength <= maxLength)
                        break;

                    if (end < filenameIndex)
                        totalLength -= elements[++end].Length - 1;

                    if (begin == 0 && end == filenameIndex)
                        break;
                }

                // assemble final string

                for (int i = 0; i < begin; i++)
                {
                    root += elements[i] + '\\';
                }

                root += "...\\";

                for (int i = end; i < filenameIndex; i++)
                {
                    root += elements[i] + '\\';
                }

                return root + elements[filenameIndex];
            }
            return pathname;
        }

        void LoadRecentFiles()
        {
            recentFiles = LoadRecentFilesCore();
        }

        List<RecentFile> LoadRecentFilesCore()
        {
            List<string> list = RecentFiles;

            List<RecentFile> files = new List<RecentFile>(list.Count);

            int i = 0;
            foreach (string filepath in list)
                files.Add(new RecentFile(i++, filepath));

            return files;
        }

        private class RecentFile
        {
            public readonly int Number;
            public readonly string Filepath;
            public MenuItem MenuItem;

            public string DisplayPath
            {
                get
                {
                    return Path.Combine(
                        Path.GetDirectoryName(Filepath),
                        Path.GetFileNameWithoutExtension(Filepath));
                }
            }

            public RecentFile(int number, string filepath)
            {
                Number = number;
                Filepath = filepath;
            }
        }

        public class MenuClickEventArgs : EventArgs
        {
            public string Filepath { get; private set; }

            public MenuClickEventArgs(string filepath)
            {
                Filepath = filepath;
            }
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            OnMenuClick(menuItem);
        }

        protected virtual void OnMenuClick(MenuItem menuItem)
        {
            string filepath = GetFilepath(menuItem);

            if (String.IsNullOrEmpty(filepath)) return;

            EventHandler<MenuClickEventArgs> dMenuClick = MenuClick;
            if (dMenuClick != null) dMenuClick(menuItem, new MenuClickEventArgs(filepath));
        }

        string GetFilepath(MenuItem menuItem)
        {
            foreach (RecentFile r in recentFiles)
                if (r.MenuItem == menuItem)
                    return r.Filepath;

            return String.Empty;
        }

        //-----------------------------------------------------------------------------------------

        public static class ApplicationAttributes
        {
            static readonly AssemblyTitleAttribute title;
            static readonly AssemblyCompanyAttribute company;
            static readonly AssemblyCopyrightAttribute copyright;
            static readonly AssemblyProductAttribute product;

            private static string Title { get; set; }
            public static string CompanyName { get; private set; }
            private static string Copyright { get; set; }
            public static string ProductName { get; private set; }

            static readonly Version version;
            private static string Version { get; set; }

            static ApplicationAttributes()
            {
                try
                {
                    Title = String.Empty;
                    CompanyName = String.Empty;
                    Copyright = String.Empty;
                    ProductName = String.Empty;
                    Version = String.Empty;

                    var assembly = Assembly.GetEntryAssembly();

                    if (assembly != null)
                    {
                        object[] attributes = assembly.GetCustomAttributes(false);

                        foreach (object attribute in attributes)
                        {
                            Type type = attribute.GetType();

                            if (type == typeof(AssemblyTitleAttribute)) title = (AssemblyTitleAttribute)attribute;
                            if (type == typeof(AssemblyCompanyAttribute)) company = (AssemblyCompanyAttribute)attribute;
                            if (type == typeof(AssemblyCopyrightAttribute)) copyright = (AssemblyCopyrightAttribute)attribute;
                            if (type == typeof(AssemblyProductAttribute)) product = (AssemblyProductAttribute)attribute;
                        }

                        version = assembly.GetName().Version;
                    }

                    if (title != null) Title = title.Title;
                    if (company != null) CompanyName = company.Company;
                    if (copyright != null) Copyright = copyright.Copyright;
                    if (product != null) ProductName = product.Product;
                    if (version != null) Version = version.ToString();
                }
                catch { }
            }
        }

        //-----------------------------------------------------------------------------------------
    }
}
