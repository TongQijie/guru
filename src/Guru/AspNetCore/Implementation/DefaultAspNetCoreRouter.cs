using System.Text.RegularExpressions;
using Guru.AspNetCore.Abstractions;
using Guru.AspNetCore.Configuration;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Attributes;
using Guru.ExtensionMethod;

namespace Guru.AspNetCore.Implementation
{
    [Injectable(typeof(IAspNetCoreRouter), Lifetime.Singleton)]
    internal class DefaultAspNetCoreRouter : IAspNetCoreRouter
    {
        public void GetRouteData(CallingContext context)
        {
            if (!context.InputParameters.ContainsKey("RequestPath"))
            {
                return;
            }

            var router = context.ApplicationConfiguration?.Router;

            var requestPath = context.InputParameters.Get("RequestPath").Value.Trim('/');
            if (requestPath == null || !requestPath.HasValue())
            {
                if (router == null || !router.Default.HasValue())
                {
                    return;
                }

                context.RouteData = router.Default.SplitByChar('/');
                return;
            }

            if (router.RewriteRules.HasLength())
            {
                foreach (var rule in router.RewriteRules)
                {
                    if (Regex.IsMatch(requestPath, rule.Pattern, RegexOptions.IgnoreCase))
                    {
                        if (rule.Mode == RewriteMode.Override)
                        {
                            requestPath = rule.Value;
                            break;
                        }
                        else if (rule.Mode == RewriteMode.Replace)
                        {
                            requestPath = Regex.Replace(requestPath, rule.Pattern, rule.Value);
                            break;
                        }
                    }
                }
            }

            context.RouteData = requestPath.SplitByChar('/');
        }
    }
}