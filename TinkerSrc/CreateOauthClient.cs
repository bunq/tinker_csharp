using System;
using System.Collections.Generic;
using System.IO;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Json;
using Bunq.Sdk.Model.Core;
using Bunq.Sdk.Model.Generated.Endpoint;
using Mono.Options;
using Tinker.Utils;

namespace TinkerSrc
{
    public class CreateOauthClient : ITinker
    {
        /// <summary>
        /// API constants.
        /// </summary>
        protected const string OPTION_CONTEXT = "context=";
        protected const string OPTION_REDIRECT_URI = "redirect=";

        /**
         * File constants.
         */
        protected const string FILE_OAUTH_CONFIGURATION = "oauth.conf";

        /**
         * Error constants.
         */
        protected const string ERROR_MISSING_MANDATORY_OPTION = "Missing mandatory option.";
            
        public void Run(string[] args)
        {
            Dictionary<string, string> allOption = AssertMandatoryOptions(args);

            BunqContext.LoadApiContext(
                ApiContext.Restore(allOption[OPTION_CONTEXT])
            );

            OauthClient oauthClient;
            if (File.Exists(FILE_OAUTH_CONFIGURATION))
            {
                oauthClient = OauthClient.CreateFromJsonString(File.ReadAllText(FILE_OAUTH_CONFIGURATION));
            }
            else
            {
                int oauthClientId = OauthClient.Create().Value;

                OauthCallbackUrl.Create(oauthClientId, allOption[OPTION_REDIRECT_URI]);
                oauthClient = OauthClient.Get(oauthClientId).Value;
                
                String serializedClient = BunqJsonConvert.SerializeObject(oauthClient);
                File.WriteAllText(FILE_OAUTH_CONFIGURATION, serializedClient);
            }

            OauthAuthorizationUri authorizationUri = OauthAuthorizationUri.Create(
                OauthResponseType.CODE,
                allOption[OPTION_REDIRECT_URI],
                oauthClient
            );
            
            Console.WriteLine(" | Created oauth client. Stored in {0}.", FILE_OAUTH_CONFIGURATION);
            Console.WriteLine(" | Point your user to {0} to obtain an Authorization code.", authorizationUri.AuthorizationUri);
        }
        
        private Dictionary<string, string> AssertMandatoryOptions(string[] allArgument)
        {
            Dictionary<string, string> allOption = new Dictionary<string, string>();

            new OptionSet
            {
                {OPTION_CONTEXT, value => allOption.Add(OPTION_CONTEXT, value)},
                {OPTION_REDIRECT_URI, value => allOption.Add(OPTION_REDIRECT_URI, value)}
            }.Parse(allArgument);
            
            if (allOption[OPTION_CONTEXT] != null &&
                allOption[OPTION_REDIRECT_URI] != null) {
                return allOption;
            }
            
            throw new BunqException(ERROR_MISSING_MANDATORY_OPTION);
        }
    }
}