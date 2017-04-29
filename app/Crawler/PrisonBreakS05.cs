using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Guru.Jobs;
using Guru.Network;
using Guru.DependencyInjection;
using Guru.Network.Abstractions;
using Guru.Logging.Abstractions;
using Guru.Formatter.Abstractions;

using HtmlAgilityPack;

namespace Crawler
{
    public class PrisonBreakS05 : AbstractJob
    {
        private readonly IHttpClientRequest _Request;

        private readonly ILogger _Logger;

        private readonly IFormatter _Formatter;

        public PrisonBreakS05()
            : base("PrisonBreakS05", new Schedule()
            {
                Cycle = ExecutionCycle.Periodic,
                Point = new ExecutionPoint()
                {
                    Second = 10,
                },
            })
        {
            _Request = Container.Resolve<IHttpClientBroker>()
                .Get(new DefaultHttpClientSettings("PrisonBreakS05"));

            _Logger = Container.Resolve<IFileLogger>();

            _Formatter = Container.Resolve<IJsonFormatter>();
        }

        protected override void OnRun(string[] args)
        {
            throw new NotImplementedException();
        }

        protected override async Task OnRunAsync(string[] args)
        {
            var doc = new HtmlDocument();

            using (var response = await _Request.GetAsync($"http://cn163.net/archives/23645"))
            {
                if (response.StatusCode == 200)
                {
                    using (var stream = await response.GetStream())
                    {
                        doc.Load(stream);
                    }
                }
            }

            var ps = doc.DocumentNode.SelectNodes("//div[@id='entry']/p");
            if (ps == null || ps.Count == 0)
            {
                _Logger.LogEvent("PrisonBreakS05", Severity.Warning, "p not found.");
                return;
            }

            var anchors = new List<HtmlNode>();
            foreach (var p in ps)
            {
                var nodes = p.SelectNodes("./a");
                if (nodes == null)
                {
                    continue;
                }

                anchors.AddRange(nodes);
            }

            var downloadLinks = new List<DownloadLink>();
            foreach (var anchor in anchors)
            {
                var href = anchor.Attributes["href"].Value;
                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }

                if (href.StartsWith("thunder://") || href.StartsWith("ed2k://"))
                {
                    downloadLinks.Add(new DownloadLink()
                    {
                        Title = "PrisonBreakS05",
                        LinkText = anchor.InnerText,
                        LinkAddr = href,
                        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
            }

            _Logger.LogEvent("PrisonBreakS05", Severity.Information, $"found {downloadLinks.Count} download link(s).");

            if (downloadLinks.Count > 0)
            {
                _Logger.LogEvent("PrisonBreakS05", Severity.Information, await _Formatter.WriteStringAsync(downloadLinks, Encoding.UTF8));

                RedisClient.Current.Set("DownloadLinks.PrisonBreakS05", await _Formatter.WriteStringAsync(downloadLinks, Encoding.UTF8), null);
            }
        }
    }
}