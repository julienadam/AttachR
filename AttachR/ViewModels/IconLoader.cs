using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AttachR.ViewModels
{
    public static class IconLoader
    {
        public static ImageSource LoadIcon(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var ico = Icon.ExtractAssociatedIcon(path);
                if (ico == null)
                {
                    return null;
                }

                try
                {
                    using (ico)
                    {
                        return Imaging.CreateBitmapSourceFromHIcon(
                            ico.Handle,
                            new Int32Rect(0, 0, ico.Width, ico.Height),
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}