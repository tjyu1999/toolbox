using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

class Program {
    static async Task DownloadFile(string url, string fname) {
        using (HttpClient client = new HttpClient()) {
            var r = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var size = r.Content.Headers.ContentLength ?? 0;
            var curr = 0;
            var buffer = new byte[1024];

            using (var file_stream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.None)) {
                Console.WriteLine("Progress: [  0%] []");

                using (var content = await r.Content.ReadAsStreamAsync()) {
                    int data;

                    while ((data = await content.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                        await file_stream.WriteAsync(buffer, 0, data);
                        curr += data;

                        var pctg = (curr / (double)size) * 100;
                        var bar_length = 150;
                        var bar = new string('#', (int)(pctg / (100.0 / bar_length))) + new string('.', bar_length - (int)(pctg / (100.0 / bar_length)));
                        Console.Write($"\rProgress: [{pctg:3.0f}%] [{bar}]");
                    }
                }
            }
        }

        Console.WriteLine("\n");
    }

    static void ExtractFile(string fname, string dst) {
        if (!Directory.Exists(dst))
            Directory.CreateDirectory(dst);

        ZipFile.ExtractToDirectory(fname, dst);
    }

    static async Task Main(string[] args) {
        if (args.Length < 2) {
            Console.WriteLine("The command should be \"dotnet run {url} {fname}\"");
            return;
        }

        string url = args[0];
        string fname = args[1] + ".zip";
        string dst = args[1];

        await DownloadFile(url, fname);
        ExtractFile(fname, dst);

        Console.WriteLine("Download and extraction are completed.");
    }
}
