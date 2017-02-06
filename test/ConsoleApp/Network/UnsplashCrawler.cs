using System;
using System.IO;
using System.Threading.Tasks;

using Guru.Network;
using Guru.Formatter.Json;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;

namespace ConsoleApp.Network
{
    public class UnsplashCrawler
    {
        public void Run()
        {
            InternalRun().GetAwaiter().GetResult();
        }

        public async Task InternalRun()
        {
            for (int i = 1; i <= 100; i++)
            {
                Photo[] photos = null;
                var retryTimes = 0;
                while (retryTimes < 10 && (photos = await GetItemsPerPage(i)) == null)
                {
                    retryTimes++;
                }
                if (photos == null || photos.Length == 0)
                {
                    return;
                }

                if (!"./downloads".IsFolder())
                {
                    Directory.CreateDirectory("./downloads".FullPath());
                }

                foreach (var photo in photos.Subset(x => x.Links != null && x.Links.Download != null))
                {
                    if ($"./downloads/{photo.Id}.jpg".IsFile())
                    {
                        continue;
                    }

                    retryTimes = 0;
                    while (retryTimes < 3 && !await DownloadImage(photo.Links.Download, $"./downloads/{photo.Id}.jpg".FullPath()))
                    {
                        File.Delete($"./downloads/{photo.Id}.jpg".FullPath());
                        retryTimes++;
                    }
                }
            }
        }

        private async Task<Photo[]> GetItemsPerPage(int page)
        {
            var uri = string.Format("https://unsplash.com/napi/photos/curated?page={0}&order_by=latest", page);

            using (var broker = new HttpBroker(uri))
            {
                broker.SetHeader("authorization", "Client-ID d69927c7ea5c770fa2ce9a2f1e3589bd896454f7068f689d8e41a25b54fa6042");

                Console.Write($":) start to fetch page {page}... ");

                if (await broker.GetAsync() == 200)
                {
                    Console.WriteLine("succeeded");

                    return await broker.GetBodyAsync<Photo[], IJsonFormatter>();
                }
                else
                {
                    Console.WriteLine("failed");
                }
            }

            return null;
        }

        private async Task<bool> DownloadImage(string uri, string path)
        {
            using (var broker = new HttpBroker(uri))
            {
                Console.Write($":) start to download from {uri}... ");

                if (await broker.GetAsync() == 200)
                {
                    Console.WriteLine("succeeded");

                    var size = broker.GetResponseHeader<int>("Content-Length");

                    using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        var total = 0;

                        await broker.GetBodyAsync(async (b, o, c) =>
                        {
                            await outputStream.WriteAsync(b, 0, c);

                            total += c;
                            Console.Write($"\r:) downloaded: {total} bytes, {(int)(total * 100.0 / size)}%");
                        }, 50 * 1024);

                        Console.WriteLine();
                    }

                    return true;
                }
                else
                {
                    Console.WriteLine("failed");
                }
            }

            return false;
        }

        public class Photo
        {
            [JsonProperty(Alias = "id")]
            public string Id { get; set; }

            [JsonProperty(Alias = "links")]
            public Links Links { get; set; }
        }

        public class Links
        {
            [JsonProperty(Alias = "download")]
            public string Download { get; set; }
        }
    }
}