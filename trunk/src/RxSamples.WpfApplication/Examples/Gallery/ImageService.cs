using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RxSamples.WpfApplication.Examples.Gallery
{
    public interface IImageService
    {
        IEnumerable<string> EnumerateImages();
    }

    public class ImageService : IImageService
    {
        //protected static readonly string RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        protected static readonly string RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);

        private static bool ImageFilter(string value)
        {
            string extension = value.ToLower().Substring(value.Length - 4);
            switch (extension)
            {
                case ".bmp":
                case ".gif":
                case ".jpg":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }

        public IEnumerable<string> EnumerateImages()
        {
            return Directory.EnumerateFiles(RootFolder, "*.*", SearchOption.AllDirectories)
                            .Where(ImageFilter)
                            .Select(value =>
                                {
                                    Thread.Sleep(50);
                                    return value;
                                });
        }
    }
}