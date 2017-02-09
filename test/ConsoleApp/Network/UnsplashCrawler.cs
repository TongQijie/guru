using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Network;
using Guru.Formatter.Json;
using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

namespace ConsoleApp.Network
{
    public class UnsplashCrawler
    {
        private readonly IHttpClientBroker _Broker;

        private readonly IHttpClientRequest _Request;

        public UnsplashCrawler()
        {
            _Broker = ContainerEntry.Resolve<IHttpClientBroker>();

            _Request = _Broker.Get(new DefaultHttpClientSettings(
                "unsplash",
                new Dictionary<string, string[]>()
                {
                    { "authorization", new string[] { "Client-ID d69927c7ea5c770fa2ce9a2f1e3589bd896454f7068f689d8e41a25b54fa6042" } }
                },
                new DefaultWebProxy("s1firewall", 8080, "jt69", "Newegg@12345", "buyabs.corp"),
                null));
        }

        public void Run()
        {
            InternalRun("D:\\demo\\crawler\\unsplash.com\\Unsplash.ConsoleApp\\bin\\Debug\\images".FullPath()).GetAwaiter().GetResult();
        }

        private async Task InternalRun(string folder)
        {
            for (int i = 1; i <= 200; i++)
            {
                Photo[] photos = null;
                var retryTimes = 0;
                while (retryTimes < 10 && (photos = await GetItemsPerPage(i)) == null)
                {
                    retryTimes++;
                }
                if (photos == null || photos.Length == 0)
                {
                    Console.WriteLine($"exit at page {i}.");
                    break;
                }

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                foreach (var photo in photos.Subset(x => x.Links != null && x.Links.Download != null))
                {
                    var path = Path.Combine(folder, $"{photo.Id}.jpg");
                    if (File.Exists(path))
                    {
                        continue;
                    }

                    retryTimes = 0;
                    while (retryTimes < 3 && !await DownloadImage(photo.Links.Download, path))
                    {
                        File.Delete(path);
                        retryTimes++;
                    }
                }
            }
        }

        private async Task<Photo[]> GetItemsPerPage(int page)
        {
            var uri = string.Format("https://unsplash.com/napi/photos/curated?page={0}&order_by=latest", page);

            Console.Write($":) start to fetch page {page}... ");

            using (var response = await _Request.GetAsync(uri))
            {
                if (response.StatusCode == 200)
                {
                    Console.WriteLine("succeeded");

                    return await response.GetBodyAsync<Photo[], IJsonFormatter>();
                }
                else
                {
                    Console.WriteLine($"failed, status code: {response.StatusCode}");
                }
            }

            return null;
        }

        private async Task<bool> DownloadImage(string uri, string path)
        {
            Console.Write($":) start to download from {uri}... ");

            using (var response = await _Request.GetAsync(uri))
            {
                if (response.StatusCode == 200)
                {
                    Console.WriteLine("succeeded");

                    var size = int.Parse(response.Headers["Content-Length"][0]);

                    using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        var total = 0;

                        await response.GetBodyAsync(async (b, o, c) =>
                        {
                            await outputStream.WriteAsync(b, 0, c);

                            total += c;
                            Console.Write($"\r:) downloaded: {total} bytes, {(int)(total * 100.0 / size)}%");
                        }, 50 * 1024);

                        Console.WriteLine();
                    }
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