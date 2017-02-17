using System.Text.RegularExpressions;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Middleware.Abstractions;
using Guru.Middleware.Configuration;
using Guru.DependencyInjection.Abstractions;

namespace Guru.Middleware.Components
{
    [DI(typeof(IUriRewriteComponent), Lifetime = Lifetime.Singleton)]
    public class UriRewriteComponent : IUriRewriteComponent
    {
        private readonly IFileManager _FileManager;

        public UriRewriteComponent(IFileManager fileManager)
        {
            _FileManager = fileManager;
        }

        public string Rewrite(string uri)
        {
            var rules = _FileManager.Single<IApplicationConfiguration>().Rewrites;
            if (rules.HasLength())
            {
                foreach (var rule in rules)
                {
                    if (Regex.IsMatch(uri, rule.Pattern, RegexOptions.IgnoreCase))
                    {
                        if (rule.Mode == RewriteMode.Override)
                        {
                            return rule.Value;
                        }
                        else if (rule.Mode == RewriteMode.Replace)
                        {
                            return Regex.Replace(uri, rule.Pattern, rule.Value);
                        }
                    }
                }
            }

            return uri;
        }
    }
}