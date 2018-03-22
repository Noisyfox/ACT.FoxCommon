using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ACT.FoxCommon.core;
using Newtonsoft.Json.Linq;

namespace ACT.FoxCommon.update
{

    public abstract class UpdateCheckerBase<TMainController, TPlugin> : IPluginComponentBase<TMainController, TPlugin>
        where TPlugin : PluginBase<TMainController>
        where TMainController : MainControllerBase
    {
        protected abstract string UpdateUrl { get; }

        private readonly UpdateCheckerThread _workingThread = new UpdateCheckerThread();
        private TMainController _controller;

        public void AttachToAct(TPlugin plugin)
        {
            _controller = plugin.Controller;
        }

        public void PostAttachToAct(TPlugin plugin)
        {
        }

        public void CheckUpdate(bool forceNotify)
        {
            _workingThread.StartWorkingThread(new UpdateContext
            {
                Service = this,
                Controller = _controller,
                ForceNotify = forceNotify
            });
        }

        public void Stop()
        {
            _workingThread.StopWorkingThread();
        }

        protected abstract string ParseVersion(string fileName);


        private class UpdateContext
        {
            public UpdateCheckerBase<TMainController, TPlugin> Service { get; set; }
            public TMainController Controller { get; set; }
            public bool ForceNotify { get; set; }
        }

        private class UpdateCheckerThread : BaseThreading<UpdateContext>
        {
            private const string UserAgent =
                    "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.3319.102 Safari/537.36"
                ;

            protected override void DoWork(UpdateContext context)
            {
                context.Controller.NotifyUpdateCheckingStarted(false);

                try
                {
                    //specify to use TLS 1.2 as default connection
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    string responseBody;
                    using (var client = new HttpClient())
                    using (var request = new HttpRequestMessage())
                    {
                        request.Method = HttpMethod.Get;
                        request.RequestUri = new Uri(context.Service.UpdateUrl);
                        request.Headers.Add("User-Agent", UserAgent);

                        var response = client.SendAsync(request).Result;
                        response.EnsureSuccessStatusCode();
                        responseBody = response.Content.ReadAsStringAsync().Result;
                    }
                    var result = JArray.Parse(responseBody);
                    var versions = new List<PublishVersion>();

                    foreach (var release in result.Cast<JObject>())
                    {
                        var isPrerelease = (bool) release["prerelease"];
                        var message = (string) release["body"];
                        var page = (string) release["html_url"];
                        foreach (var asset in ((JArray) release["assets"]).Cast<JObject>())
                        {
                            var name = (string) asset["name"];

                            // Parse name
                            var version = context.Service.ParseVersion(name);
                            if (version != null)
                            {
                                if (Version.TryParse(version, out var pv))
                                {
                                    versions.Add(new PublishVersion
                                    {
                                        IsPreRelease = isPrerelease,
                                        RawVersion = version,
                                        ParsedVersion = pv,
                                        ReleaseMessage = message,
                                        PublishPage = page,
                                    });
                                }
                            }
                        }
                    }

                    // Sort
                    versions.Sort((l, r) =>
                    {
                        var c = r.ParsedVersion.CompareTo(l.ParsedVersion);
                        return c != 0 ? c : r.IsPreRelease.CompareTo(l.IsPreRelease);
                    });

                    var latest = versions.FirstOrDefault();
                    var latestStable = versions.FirstOrDefault(it => !it.IsPreRelease);
                    context.Controller.NotifyVersionChecked(false, new VersionInfo
                    {
                        LatestVersion = latest,
                        LatestStableVersion = latestStable
                    }, context.ForceNotify);
                }
                catch (Exception ex)
                {
                    context.Controller.NotifyVersionChecked(false, null, context.ForceNotify);
                    context.Controller.NotifyLogMessageAppend(false, ex + "\n");
                }
            }
        }

    }
}
