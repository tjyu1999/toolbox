using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

class Program {
    static async Task DownloadFile(string url, string fname) {
        using (HttpClient client = new HttpClient()) {
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            long size = response.Content.Headers.ContentLength ?? 0;
            double sizeInMB = size / (1024.0 * 1024.0);

            Console.WriteLine($"[info] {fname} {sizeInMB:0.00}MB");
            Console.WriteLine($"[download] Destination: {Directory.GetCurrentDirectory()}/{fname}");

            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var fileStream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var contentStream = await response.Content.ReadAsStreamAsync()) {
                byte[] buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;

                    double progress = totalRead / (double)size * 100;
                    Console.Write($"\r[download] Progress: {progress:0.00}%");
                }
            }

            stopwatch.Stop();
            double downloadTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
            string formattedTime = TimeSpan.FromSeconds(downloadTimeInSeconds).ToString(@"hh\:mm\:ss");
            double downloadSpeed = sizeInMB / downloadTimeInSeconds;

            Console.WriteLine($"\n[download] 100% of {sizeInMB:0.00}MB in {formattedTime} at {downloadSpeed:0.00}MB/s");
        }
    }

    static void ExtractFile(string fname, string fdir) {
        if (!Directory.Exists(fdir)) {
            Directory.CreateDirectory(fdir);
        }

        Console.WriteLine($"[extract] Destination: {Directory.GetCurrentDirectory()}/{fdir}");

        Stopwatch stopwatch = Stopwatch.StartNew();

        using (ZipArchive archive = ZipFile.OpenRead(fname)) {
            long totalSize = 0;
            foreach (var entry in archive.Entries) {
                totalSize += entry.Length;
            }
            double sizeInMB = totalSize / (1024.0 * 1024.0);

            foreach (var entry in archive.Entries) {
                string destinationPath = Path.Combine(fdir, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath) ?? string.Empty);
                entry.ExtractToFile(destinationPath, true);
            }

            stopwatch.Stop();
            double extractTimeInSeconds = stopwatch.Elapsed.TotalSeconds;
            string formattedTime = TimeSpan.FromSeconds(extractTimeInSeconds).ToString(@"hh\:mm\:ss");
            double extractSpeed = totalSize / extractTimeInSeconds / (1024.0 * 1024.0);

            Console.WriteLine($"[extract] 100% of {sizeInMB:0.00}MB in {formattedTime} at {extractSpeed:0.00}MB/s");
        }
    }

    static async Task Main(string[] args) {
        if (args.Length < 1) {
            Console.WriteLine("Usage: <url>");
            return;
        }

        string url = args[0];
        string fname = $"{DateTime.Now:ddHHmmss}.zip";
        string fdir = $"{DateTime.Now:ddHHmmss}";

        await DownloadFile(url, fname);
        ExtractFile(fname, fdir);
    }
}
