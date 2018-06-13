using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DateTimeFixer
{
    public class FileHitsInfo
    {
        public FileInfo File { get; set; } = null;
        public DateTime SuggestedNewDate { get; set; } = DateTime.MinValue;
    }

    public static class Program
    {
        public static DateTime FallbackDateTime = DateTime.Now.AddYears(-2).Date;
        public static DateTime NewestDateAllowed = DateTime.Now.Date.AddDays(-1);

        public static void Main(string[] args)
        {
            var result = new List<FileHitsInfo>();

            foreach (var arg in args)
            {
                if (!Directory.Exists(arg))
                    continue;

                Console.WriteLine($"Scanning {arg} for files from the future!");
                var subRes = WalkFolder(new DirectoryInfo(arg));
                if (subRes.Count > 0)
                    result.AddRange(subRes);
            }

            foreach (var res in result)
            {
                Console.WriteLine($"Found file from future: '{res.File.FullName}' - (Date: {res.File.LastWriteTime}, New suggested date: ({res.SuggestedNewDate})");
                res.File.LastWriteTime = res.SuggestedNewDate;
                res.File.Refresh();
            }
        }

        private static List<FileHitsInfo> WalkFolder(DirectoryInfo dir)
        {
            var retVal = new List<FileHitsInfo>();

            foreach (var child in dir.GetDirectories())
            {
                var files = new FileInfo[0];
                try
                {
                    files = child.GetFiles();
                }
                catch (UnauthorizedAccessException)
                {
                    // This will be triggered by a path like "System Volume Information" for example...
                    // We just skip this folder and continue.
                    continue;
                }

                var oldestFile = files.OrderBy(x => x.LastWriteTime)
                    .FirstOrDefault();

                var newDateToUse = FallbackDateTime;

                if (oldestFile != null)
                {
                    if (oldestFile.LastWriteTime < NewestDateAllowed)
                        newDateToUse = oldestFile.LastWriteTime;
                }

                var filesFromFuture = files.Where(x => x.LastWriteTime >= NewestDateAllowed);
                foreach (var fi in filesFromFuture)
                    retVal.Add(new FileHitsInfo { File = fi, SuggestedNewDate = newDateToUse });

                var childResults = WalkFolder(child);
                if (childResults.Count > 0)
                    retVal.AddRange(childResults);
            }

            return retVal;
        }
    }
}
