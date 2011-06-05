using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RxSamples.ConsoleApp
{
    public class ReadingDisk
    {
        public void FolderSizes()
        {
            var timer = Stopwatch.StartNew();
            FolderSizes_Task();
            timer.Stop();
            using (new ConsoleColor(System.ConsoleColor.Red))
            {
                Console.WriteLine("FolderSizes_Task(); {0}", timer.Elapsed);//0:55
            }
            
            timer = Stopwatch.StartNew();
            FolderSizes_Synchronous();
            timer.Stop();
            using (new ConsoleColor(System.ConsoleColor.Red))
            {
                Console.WriteLine("FolderSizes_Synchronous(); {0}", timer.Elapsed); //1:17
            }
        }

        public void FolderSizes_Task()
        {
            string path = @"C:\";
            var directories = Directory.EnumerateDirectories(path);
            Task.WaitAll(
                directories.Select(dir => 
                    Task.Factory
                        .StartNew(() => DirectorySize(dir), TaskCreationOptions.LongRunning)
                        .ContinueWith(size => WriteDirSize(dir, size.Result))
                ).ToArray());
        }

        public void FolderSizes_Synchronous()
        {
            string path = @"C:\";
            var directories = Directory.EnumerateDirectories(path);
            foreach (var directory in directories)
            {
                var directoryBytes = DirectorySize(directory);

                WriteDirSize(directory, directoryBytes);
            }
        }

        private static long DirectorySize(string path)
        {
            long directoryBytes = 0L;
            try
            {
                var dir = new DirectoryInfo(path);
                if ((dir.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint) return directoryBytes;

                var files = Directory.GetFiles(path);
                directoryBytes += files.Sum(file => FileSize(file));

                var subDirectories = Directory.EnumerateDirectories(path);
                directoryBytes += subDirectories.Sum(directory => DirectorySize(directory));
            }
            catch (UnauthorizedAccessException){}

            return directoryBytes;
        }

        private static void WriteDirSize(string directory, long directoryBytes)
        {
            var size = FormatSize(directoryBytes);
            size = (size + new string(' ', 20)).Substring(0, 20);
            Console.WriteLine("{1} {0}", directory, size);
        }

        private static long FileSize(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch (Exception)
            {
                return 0l;
            }
        }

        private static string FormatSize(long bytes)
        {
            double contentLength = (double)bytes;
            var culture = CultureInfo.CurrentCulture;
            string retval;
            if (contentLength < 921)                //less than 0.9KB
            {
                retval = contentLength.ToString(culture);
                retval += "B";
            }
            else if (contentLength < 943718)        //less than 0.9MB
            {
                retval = (Math.Ceiling(contentLength / 1024d).ToString(culture));
                retval += "KB";
            }
            else if (contentLength < 966367641)     //less than 0.9GB
            {
                retval = (Math.Ceiling(contentLength / 1048576d).ToString(culture));        //1048576 = 1024^2;
                retval += ("MB");
            }
            else if (contentLength < 989560464998)  //less than 0.9TB
            {
                retval = (Math.Ceiling(contentLength / 1073741824d).ToString(culture));     //1073741824 = 1024^3;
                retval += ("GB");
            }
            else
            {
                retval = (Math.Ceiling(contentLength / 1099511627776d).ToString(culture));     //1099511627776 = 1024^4;
                retval += ("TB");
            }
            return retval;
        }
    }
}
