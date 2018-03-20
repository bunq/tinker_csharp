using System;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Http;
using Mono.Options;
using Tinker.Utils;

namespace TinkerSrc
{
    public class CreateProductionConfiguration : ITinker
    {
        public void Run(string[] args)
        {
            string apiKey = null;
            
            new OptionSet
            {
                {"api-key=", v => apiKey = v }
            }.Parse(args);

            if (apiKey == null) throw new BunqException("Argument api-key is required.");

            var apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION, apiKey, Environment.MachineName);
            apiContext.Save("bunq-production.conf");
        }
    }
}
