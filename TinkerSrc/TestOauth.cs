using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Model.Core;
using Bunq.Sdk.Model.Generated.Endpoint;
using Mono.Options;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class TestOauth : ITinker
    {
        /// <summary>
        /// Option constants.
        /// </summary>
        protected const string  OPTION_AUTH_CODE = "code=";
        protected const string  OPTION_CLIENT_CONFIGURATION = "configuration=";
        protected const string  OPTION_REDIRECT = "redirect=";
        protected const string  OPTION_CONTEXT = "context=";

        /// <summary>
        /// API constants.
        /// </summary>
        protected const string API_DEVICE_DESCRIPTION = "##### YOUR DEVICE DESCRIPTION #####";

        /// <summary>
        /// Error constants.
        /// </summary>
        protected const string ERROR_MISSING_MANDATORY_OPTION = "Missing mandatory option.";

        public void Run(string[] args)
        {
            Dictionary<string, string> allOption = AssertMandatoryOptions(args);
            
            ApiContext apiContext = ApiContext.Restore(allOption[OPTION_CONTEXT]);
            BunqContext.LoadApiContext(apiContext);
            
            OauthAccessToken accessToken = OauthAccessToken.Create(
                OauthGrantType.AUTHORIZATION_CODE,
                allOption[OPTION_AUTH_CODE],
                allOption[OPTION_REDIRECT],
                CreateOauthClientFromFile(allOption[OPTION_CLIENT_CONFIGURATION])
            );

            apiContext = CreateApiContextByOauthToken(
                accessToken,
                ShareLib.DetermineEnvironmentType(args)
            );
            BunqContext.LoadApiContext(apiContext);

            (new UserOverview()).Run(new String[]{});
        }
        
           
        private Dictionary<string, string> AssertMandatoryOptions(string[] allArgument)
        {
            Dictionary<string, string> allOption = new Dictionary<string, string>();

            new OptionSet
            {
                {OPTION_AUTH_CODE, value => allOption.Add(OPTION_AUTH_CODE, value)},
                {OPTION_REDIRECT, value => allOption.Add(OPTION_REDIRECT, value)},
                {OPTION_CONTEXT, value => allOption.Add(OPTION_CONTEXT, value)},
                {OPTION_CLIENT_CONFIGURATION, value => allOption.Add(OPTION_CLIENT_CONFIGURATION, value)}
            }.Parse(allArgument);
            
            if (allOption[OPTION_AUTH_CODE] != null &&
                allOption[OPTION_REDIRECT] != null &&
                allOption[OPTION_CONTEXT] != null &&
                allOption[OPTION_CLIENT_CONFIGURATION] != null) {
                return allOption;
            }
            
            throw new BunqException(ERROR_MISSING_MANDATORY_OPTION);
        }

        private OauthClient CreateOauthClientFromFile(String path)
        {
            return OauthClient.CreateFromJsonString(
                File.ReadAllText(path)
            );
        }
        
        private static ApiContext CreateApiContextByOauthToken(OauthAccessToken token, ApiEnvironmentType environmentType)
        {
            return ApiContext.Create(
                environmentType,
                token.Token,
                API_DEVICE_DESCRIPTION
            );
        }
    }
}