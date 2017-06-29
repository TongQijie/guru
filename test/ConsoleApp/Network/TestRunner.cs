using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using ConsoleApp.Middleware;

using Guru.DependencyInjection;
using Guru.Network.Abstractions;
using Guru.Formatter.Abstractions;
using Guru.Network;
using System.IO;

namespace ConsoleApp.Network
{
    public class TestRunner
    {
        private readonly IHttpClientBroker _Broker;

        public TestRunner()
        {
            _Broker = ContainerManager.Default.Resolve<IHttpClientBroker>();
        }

        public void Run()
        {
            //HttpBrokerTest().GetAwaiter().GetResult();
            Test();
        }

        private void Test()
        {
            var request = ContainerManager.Default.Resolve<IHttpClientBroker>().Get(new DefaultHttpClientSettings("", new Dictionary<string, string[]>()
            {
                { "Accept", new string[] { "*/*" } },
                { "Accept-Encoding", new string[] { "gzip, deflate" } },
                { "Accept-Language", new string[] { "zh-CN,zh;q=0.8" } },
                { "Cache-Control", new string[] { "no-cache" } },
                { "Connection", new string[] { "keep-alive"} },
                { "Cookie", new string[] 
                    { 
                        "__cfduid=d18f9a9f3c9761b2cf9c09eaaa44749d31496024705; __gads=ID=72c75b453b8be8e5:T=1496024743:S=ALNI_MbGABj1BRE6_yYIeRIh1UDmCt_pxw; WSS_FullScreenMode=false; _ga=GA1.2.1549730488.1496024712; _gid=GA1.2.1387626037.1496024800; visitor_id123902=560222809; visitor_id123902-hash=8fbd291a15224d2e2fcf67624a23b1667006811b0f07fed796f1c3aaf0bdb43eb7e2bbd78eaaa99249a73291c153d6490464c1da; __ar_v4=%7CAHW2E66F75EETC67CSYCSS%3A20170528%3A1%7CJJVGJIJFTRFWBJ2JYF2GR2%3A20170528%3A1%7CUE2CPQ6EVVBJBDHQ4S2DO6%3A20170528%3A1; _gali=ctl00_SPWebPartManager1_g_e3b09024_878e_4522_bd47_acfefd1000b0_ctl00_butSearch",
                    } 
                },
                { "Host", new string[] { "www.iata.org" } },
                { "Origin", new string[] { "http://www.iata.org" } },
                { "Pragma", new string[] { "no-cache" } },
                { "Referer", new string[] { "http://www.iata.org/publications/pages/code-search.aspx/" } },
                { "User-Agent", new string[] { "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36" } },
                { "X-MicrosoftAjax", new string[] { "Delta=true" } },
                { "X-Requested-With", new string[] { "XMLHttpRequest" } },
            }, null, null));

            request.PostAsync("", null, new Dictionary<string, string>()
            {
                { "abc", "123$$123" }
            });

            var body = "ctl00%24sm=ctl00%24sm%7Cctl00%24SPWebPartManager1%24g_e3b09024_878e_4522_bd47_acfefd1000b0%24ctl00%24butSearch&ctl00_sm_HiddenField=&ctl00%24Header%24AdvanceSearchBox%24DisplayContent%24SearchTextBox=&ctl00%24SPWebPartManager1%24g_e3b09024_878e_4522_bd47_acfefd1000b0%24ctl00%24ddlImLookingFor=ByLocationName&ctl00%24SPWebPartManager1%24g_e3b09024_878e_4522_bd47_acfefd1000b0%24ctl00%24txtSearchCriteria=ningbo&ctl00%24SPWebPartManager1%24g_e3b09024_878e_4522_bd47_acfefd1000b0%24ctl00%24txtSearchCriteriaRequiredValidatorCalloutExtender_ClientState=&__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUBMA9kFgJmD2QWAgIBD2QWBAIBD2QWBgIGD2QWAgIBDxYCHhNQcmV2aW91c0NvbnRyb2xNb2RlCymIAU1pY3Jvc29mdC5TaGFyZVBvaW50LldlYkNvbnRyb2xzLlNQQ29udHJvbE1vZGUsIE1pY3Jvc29mdC5TaGFyZVBvaW50LCBWZXJzaW9uPTE1LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPTcxZTliY2UxMTFlOTQyOWMBZAIHD2QWAmYPZBYCAgEPZBYCAgEPFgIfAAsrBAFkAhcPZBYCAgMPZBYCAgEPZBYCZg88KwAGAGQCCQ9kFgYCAw9kFgJmD2QWAgUmZ19lM2IwOTAyNF84NzhlXzQ1MjJfYmQ0N19hY2ZlZmQxMDAwYjAPZBYCZg9kFgQCAQ9kFgQCAQ8QZGQWAQICZAIFDw8WAh4EVGV4dAUIc2hhbmdoYWlkFgRmDw8WAh4PU2V0Rm9jdXNPbkVycm9yZ2RkAgEPFggeEUhpZ2hsaWdodENzc0NsYXNzBRB0ZXh0Qm94T25JbnZhbGlkHghDc3NDbGFzcwUWYWpheF9fdmFsaWRhdG9yY2FsbG91dB4TV2FybmluZ0ljb25JbWFnZVVybAU7L1N0eWxlIExpYnJhcnkvSUFUQS5vcmctdjEvQ29yZS9JbWFnZXMvSWNvbnMvaWNvbi1hbGVydC5naWYeDUNsb3NlSW1hZ2VVcmwFlAIvV2ViUmVzb3VyY2UuYXhkP2Q9TE1WZ0l1UHJTX1VJcDBWTkpESHdYMy04YnhhT0VBUTdPRDZnTUtnc2VUN0ttb3M2UVhmWGVTTWRsZFdEZW5hMjdoWE1uRHA3NjlmSjdGWHl0cmpsMzlNYndzOTFBSnVMb3pGOE5pNzVpY2F4VmhlamhKNHhicld6QlU3UkRWZzF1Vkg5MEVHcUIydjFnREhKZ2V1c0NBYloteXlYVVRhTEJnOFdTd2czanNHSGk2dHpoZm5HVGdsQVI4US1mZTBzTk1hTlVyRUw3MlZQS1d4LWFXYmVFTDk3czlyY3p6S2VuQlpMM0V2dXF4azEmdD02MzU3NDk5MjIxOTI2NDIyMTZkAgIPZBYCZg9kFgICAg8PFgIeB1Zpc2libGVnZBYGAgEPDxYCHwEFHVNlYXJjaCByZXN1bHRzIGZvciAic2hhbmdoYWkiZGQCAw8WBB8BBa8DPHRhYmxlIGNsYXNzPSJkYXRhdGFibGUiIHdpZHRoPSIxMDAlIiA%2BPHRoZWFkPjx0cj48dGQ%2BQ2l0eSBuYW1lPC90ZD48dGQ%2BMy1sZXR0ZXIgY2l0eSBjb2RlPC90ZD48dGQ%2BQWlycG9ydCBuYW1lPC90ZD48dGQ%2BMy1sZXR0ZXIgYWlycG9ydCBjb2RlPC90ZD48L3RyPjwvdGhlYWQ%2BPHRib2R5Pjx0cj48dGQ%2BU2hhbmdoYWk8L3RkPjx0ZD5TSEE8L3RkPjx0ZD5QdWRvbmcgSW50bDwvdGQ%2BPHRkPlBWRzwvdGQ%2BPC90cj48dHIgY2xhc3M9ImFsdGVybmF0ZVJvd2JhY2tncm91bmQiPjx0ZD5TaGFuZ2hhaTwvdGQ%2BPHRkPlNIQTwvdGQ%2BPHRkPk1ldHJvcG9saXRhbiBBcmVhPC90ZD48dGQ%2BU0hBPC90ZD48L3RyPjx0cj48dGQ%2BU2hhbmdoYWk8L3RkPjx0ZD5TSEE8L3RkPjx0ZD5Ib25ncWlhbyBJbnRsPC90ZD48dGQ%2BU0hBPC90ZD48L3RyPjwvdGJvZHk%2BPC90YWJsZT4eYWN0bDAwX1NQV2ViUGFydE1hbmFnZXIxX2dfZTNiMDkwMjRfODc4ZV80NTIyX2JkNDdfYWNmZWZkMTAwMGIwX2N0bDAwX2xpdFJlc3VsdHNUYWJsZVRleHRTdWJzY3JpcHRlZAIFDxYEHwEFFkRpc3BsYXlpbmcgMyByZXN1bHQocykeY2N0bDAwX1NQV2ViUGFydE1hbmFnZXIxX2dfZTNiMDkwMjRfODc4ZV80NTIyX2JkNDdfYWNmZWZkMTAwMGIwX2N0bDAwX2xpdFRvb01hbnlSZXN1bHRzVGV4dFN1YnNjcmlwdGVkAgUPZBYCAgUPZBYCAgIPZBYCAgUPZBYCAgMPFgIfB2gWAmYPZBYEAgIPZBYGAgEPFgIfB2hkAgMPFgIfB2hkAgUPFgIfB2hkAgMPDxYCHglBY2Nlc3NLZXkFAS9kZAIND2QWAgICD2QWGgIHD2QWAgIDD2QWCAIBDxYCHgRocmVmBV1odHRwOi8vd3d3LmZhY2Vib29rLmNvbS9zaGFyZXIucGhwP3U9aHR0cDovL3d3dy5pYXRhLm9yZy9wdWJsaWNhdGlvbnMvcGFnZXMvY29kZS1zZWFyY2guYXNweC9kAgMPFgIfCwVmLy93d3cubGlua2VkaW4uY29tL3NoYXJlQXJ0aWNsZT9taW5pPXRydWUmdXJsPWh0dHA6Ly93d3cuaWF0YS5vcmcvcHVibGljYXRpb25zL3BhZ2VzL2NvZGUtc2VhcmNoLmFzcHgvZAIFDxYCHwsFVC8vcGx1cy5nb29nbGUuY29tL3NoYXJlP3VybD1odHRwOi8vd3d3LmlhdGEub3JnL3B1YmxpY2F0aW9ucy9wYWdlcy9jb2RlLXNlYXJjaC5hc3B4L2QCBw8WAh8LBYMBbWFpbHRvOj9zdWJqZWN0PUkgd2FudGVkIHlvdSB0byBzZWUgdGhpcyBzaXRlJmFtcDtib2R5PUNoZWNrIG91dCB0aGlzIHNpdGUgaHR0cDovL3d3dy5pYXRhLm9yZy9wdWJsaWNhdGlvbnMvcGFnZXMvY29kZS1zZWFyY2guYXNweC9kAgsPZBYCAgEPFgIfAAsrBAFkAg0PZBYEAgEPFgIfAAsrBAFkAgMPFgIfAAsrBAFkAg8PZBYCAgEPFgIfAAsrBAFkAhMPFgIfAAsrBAFkAhcPFgIfAAsrBAFkAhkPFgIfAAsrBAFkAh0PFgIfAAsrBAFkAiEPFgIfAAsrBAFkAiMPFgIfAAsrBAFkAi0PZBYCAgEPFgQfAQXvBzxzY3JpcHQgdHlwZT0ndGV4dC9qYXZhc2NyaXB0Jz4KICB2YXIgZ29vZ2xldGFnID0gZ29vZ2xldGFnIHx8IHt9OwogIGdvb2dsZXRhZy5jbWQgPSBnb29nbGV0YWcuY21kIHx8IFtdOwogIChmdW5jdGlvbigpIHsKICAgIHZhciBnYWRzID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnc2NyaXB0Jyk7CiAgICBnYWRzLmFzeW5jID0gdHJ1ZTsKICAgIGdhZHMudHlwZSA9ICd0ZXh0L2phdmFzY3JpcHQnOwogICAgdmFyIHVzZVNTTCA9ICdodHRwczonID09IGRvY3VtZW50LmxvY2F0aW9uLnByb3RvY29sOwogICAgZ2Fkcy5zcmMgPSAodXNlU1NMID8gJ2h0dHBzOicgOiAnaHR0cDonKSArCiAgICAgICcvL3d3dy5nb29nbGV0YWdzZXJ2aWNlcy5jb20vdGFnL2pzL2dwdC5qcyc7CiAgICB2YXIgbm9kZSA9IGRvY3VtZW50LmdldEVsZW1lbnRzQnlUYWdOYW1lKCdzY3JpcHQnKVswXTsKICAgIG5vZGUucGFyZW50Tm9kZS5pbnNlcnRCZWZvcmUoZ2Fkcywgbm9kZSk7CiAgfSkoKTsKPC9zY3JpcHQ%2BCgo8c2NyaXB0IHR5cGU9J3RleHQvamF2YXNjcmlwdCc%2BCiAgZ29vZ2xldGFnLmNtZC5wdXNoKGZ1bmN0aW9uKCkgewogICAgZ29vZ2xldGFnLmRlZmluZVNsb3QoJy8zOTg3MjE1L1B1YmxpY2F0aW9ucycsIFtbMTYwLCAxNDBdLCBbMTgwLCAxNTBdXSwgJ2Rpdi1ncHQtYWQtMTQ2NDcyNjk2OTQ0My0wJykuYWRkU2VydmljZShnb29nbGV0YWcucHViYWRzKCkpOwogICAgZ29vZ2xldGFnLnB1YmFkcygpLmVuYWJsZVNpbmdsZVJlcXVlc3QoKTsKICAgIGdvb2dsZXRhZy5lbmFibGVTZXJ2aWNlcygpOwogIH0pOwo8L3NjcmlwdD48IS0tIC8zOTg3MjE1L1B1YmxpY2F0aW9ucyAtLT4KPGRpdiBpZD0nZGl2LWdwdC1hZC0xNDY0NzI2OTY5NDQzLTAnPgo8c2NyaXB0IHR5cGU9J3RleHQvamF2YXNjcmlwdCc%2BCmdvb2dsZXRhZy5jbWQucHVzaChmdW5jdGlvbigpIHsgZ29vZ2xldGFnLmRpc3BsYXkoJ2Rpdi1ncHQtYWQtMTQ2NDcyNjk2OTQ0My0wJyk7IH0pOwo8L3NjcmlwdD4KPC9kaXY%2BHkJjdGwwMF9QbGFjZUhvbGRlck1haW5fUGFnZUFjdGlvbnNfbGl0QWR2ZXJ0aXNlQ29udGVudFRleHRTdWJzY3JpcHRlZAIvD2QWAmYPFgIfB2hkAjMPZBYCAgMPFgIfAAsrBAFkGAEFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYBBTZjdGwwMCRIZWFkZXIkQWR2YW5jZVNlYXJjaEJveCREaXNwbGF5Q29udGVudCRidG5TZWFyY2iicZ4vH7jZYEf%2FAKorO74JJMx%2FUw4Dobcttjrv5wF9OA%3D%3D&__VIEWSTATEGENERATOR=BAB98CB3&_wpcmWpid=&wpcmVal=&MSOWebPartPage_PostbackSource=&MSOTlPn_SelectedWpId=&MSOTlPn_View=0&MSOTlPn_ShowSettings=False&MSOGallery_SelectedLibrary=&MSOGallery_FilterString=&MSOTlPn_Button=none&__REQUESTDIGEST=0x67809D6DAD27698B24D7309F818355E288877020D4C129F2CF21ACC359A724904F77CBDFEE34385669C4558084F785848F292859406F038940B0356367FE1867%2C29+May+2017+02%3A29%3A01+-0000&MSOSPWebPartManager_DisplayModeName=Browse&MSOSPWebPartManager_ExitingDesignMode=false&MSOWebPartPage_Shared=&MSOLayout_LayoutChanges=&MSOLayout_InDesignMode=&_wpSelected=&_wzSelected=&MSOSPWebPartManager_OldDisplayModeName=Browse&MSOSPWebPartManager_StartWebPartEditingName=false&MSOSPWebPartManager_EndWebPartEditing=false&__ASYNCPOST=true&ctl00%24SPWebPartManager1%24g_e3b09024_878e_4522_bd47_acfefd1000b0%24ctl00%24butSearch=Search";

            using (var response = request.PostAsync<ITextFormatter>("http://www.iata.org/publications/pages/code-search.aspx/", body, new Dictionary<string, string>()
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            }).GetAwaiter().GetResult())
            {
                if (response.StatusCode == 200)
                {
                    //var stream = response.GetStream().GetAwaiter().GetResult();
                    var html = response.GetBodyAsync<string>(ContainerManager.Default.Resolve<ITextFormatter>()).GetAwaiter().GetResult();
                    //document.LoadHtml(html);

                    Console.WriteLine(html.Contains("datatable"));

                    using(var outputStream = new FileStream("a.txt", FileMode.Create, FileAccess.Write))
                    {
                        using(var writer = new StreamWriter(outputStream))
                        {
                            writer.WriteLine(html);
                        }
                    }
                    //Console.WriteLine(html);
                }
            }
        }

        private async Task HttpBrokerTest()
        {
            var request = _Broker.Get();

            var host = "http://localhost:5000";

            using (var response = await request.GetAsync($"{host}/api/test/hi1"))
            {
                if (response.StatusCode == 200)
                {
                    var text = await response.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var response = await request.GetAsync(
                $"{host}/api/test/hi2", 
                new Dictionary<string, string>()
                {
                    { "welcome", "abc" }
                }))
            {
                if (response.StatusCode == 200)
                {
                    var text = await response.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var response = await request.PostAsync<IJsonFormatter>(
                $"{host}/api/test/hi3", 
                new Request() 
                { 
                    Data = "hello, world!" 
                }))
            {
                if (response.StatusCode == 200)
                {
                    var text = await response.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var response = await request.PostAsync<IJsonFormatter>(
                $"{host}/api/test/hi4", 
                new Dictionary<string, string>()
                {
                    { "word", "abc" },
                    { "welcome", "def" }
                },
                new Request() 
                { 
                    Data = "hello, world!" 
                }))
            {
                if (response.StatusCode == 200)
                {
                    var text = await response.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(text);
                }
            }

            using (var response = await request.PostAsync<IJsonFormatter>(
                $"{host}/api/test/hi5", 
                new Dictionary<string, string>()
                {
                    { "word", "abc" },
                    { "welcome", "def" },
                    { "number", "123" }
                },
                new Request() 
                { 
                    Data = "hello, world!" 
                }))
            {
                if (response.StatusCode == 200)
                {
                    var data = await response.GetBodyAsync<Response, IJsonFormatter>();

                    Console.WriteLine(data.Result);
                }
            }

            using (var response = await request.PostAsync<IJsonFormatter>(
                $"{host}/api/test/hi6", 
                new Dictionary<string, string>()
                {
                    { "word", "abc" },
                    { "welcome", "def" },
                    { "number", "123" },
                    { "price", "12.3" }
                },
                new Request() 
                { 
                    Data = "hello, world!" 
                }))
            {
                if (response.StatusCode == 200)
                {
                    var data = await response.GetBodyAsync<string, ITextFormatter>();

                    Console.WriteLine(data);
                }
            }
        }
    }
}