using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Model.Generated.Object;
using Bunq.Sdk.Security;
using Mono.Options;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class CreatePsd2Configuration : ITinker
    {
        /// <summary>
        /// API constants.
        /// </summary>
        protected const string API_DEVICE_DESCRIPTION = "##### YOUR DEVICE DESCRIPTION #####";

        /// <summary>
        /// Option constants.
        /// </summary>
        protected const string OPTION_CREDENTIALS = "credentials=";
        protected const string OPTION_CERTIFICATE_CHAIN = "chain=";
        protected const string OPTION_PASSPHRASE = "pass=";

        /// <summary>
        /// Error constants.
        /// </summary>
        protected const string ERROR_MISSING_MANDATORY_OPTION = "Missing mandatory option.";

        /// <summary>
        /// File constants.
        /// </summary>
        protected const string FILE_CONTEXT = "bunq-psd2.conf";
        
        public void Run(string[] args)
        {
            Dictionary<string, string> allOption = AssertMandatoryOptions(args);
            
            ApiContext apiContext = ApiContext.CreateForPsd2(
                ShareLib.DetermineEnvironmentType(args),
                SecurityUtils.GetCertificateFromFile(allOption[OPTION_CREDENTIALS], allOption[OPTION_PASSPHRASE]),
                new X509CertificateCollection () {
                    SecurityUtils.GetCertificateFromFile(allOption[OPTION_CERTIFICATE_CHAIN])
                },
                API_DEVICE_DESCRIPTION,
                new List<string>()
            );

            apiContext.Save(FILE_CONTEXT);
        }
        
        private Dictionary<string, string> AssertMandatoryOptions(string[] allArgument)
        {
            Dictionary<string, string> allOption = new Dictionary<string, string>();

            new OptionSet
            {
                {OPTION_CREDENTIALS, value => allOption.Add(OPTION_CREDENTIALS, value)},
                {OPTION_CERTIFICATE_CHAIN, value => allOption.Add(OPTION_CERTIFICATE_CHAIN, value)},
                {OPTION_PASSPHRASE, value => allOption.Add(OPTION_PASSPHRASE, value)}
            }.Parse(allArgument);
            
            if (allOption[OPTION_CERTIFICATE_CHAIN] != null &&
                allOption[OPTION_CREDENTIALS] != null &&
                allOption[OPTION_PASSPHRASE] != null) {
                return allOption;
            }
            
            throw new BunqException(ERROR_MISSING_MANDATORY_OPTION);
        }
    }
}