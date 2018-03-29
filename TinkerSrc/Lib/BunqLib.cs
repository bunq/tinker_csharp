using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Http;
using Bunq.Sdk.Json;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using Newtonsoft.Json.Linq;

namespace TinkerSrc.Lib
{
    public class BunqLib
    {

        private ApiEnvironmentType EnvironmentType { get; set; }

        public BunqLib(ApiEnvironmentType environmentType)
        {
            EnvironmentType = environmentType;
            SetupContext();
        }

        private void SetupContext(bool resetConfigIfNeeded = true)
        {
            if (File.Exists(DetermineBunqConfFileName()))
            {
                // Config is already present
            }
            else if (ApiEnvironmentType.SANDBOX.Equals(EnvironmentType))
            {
                var sandboxUser = GenerateNewSandboxUser();
                ApiContext.Create(ApiEnvironmentType.SANDBOX, sandboxUser.ApiKey, Environment.MachineName)
                    .Save(this.DetermineBunqConfFileName());
            }
            else
            {
                throw new BunqException("Could not find a production configuration.");
            }

            try
            {
                var apiContext = ApiContext.Restore(DetermineBunqConfFileName());
                apiContext.EnsureSessionActive();
                apiContext.Save(DetermineBunqConfFileName());

                BunqContext.LoadApiContext(apiContext);
            }
            catch (ForbiddenException forbiddenException)
            {
                if (resetConfigIfNeeded)
                {
                    HandleForbiddenExceeption(forbiddenException);
                }
                else
                {
                    throw;
                }
            }
        }

        private void HandleForbiddenExceeption(ForbiddenException forbiddenException)
        {
            if (ApiEnvironmentType.SANDBOX.Equals(EnvironmentType))
            {
                File.Delete(DetermineBunqConfFileName());
                SetupContext(false);
            }
            else
            {
                throw forbiddenException;
            }
        }

        private string DetermineBunqConfFileName()
        {
            return ApiEnvironmentType.PRODUCTION.Equals(EnvironmentType) ? BunqConnfProduction : BunqConfSandbox;
        }

        public void UpdateContext()
        {
            BunqContext.ApiContext.Save(DetermineBunqConfFileName());
        }

        public string GetCurrentUserDisplayName()
        {
            if (BunqContext.UserContext.IsOnlyUserPersonSet())
            {
                return BunqContext.UserContext.UserPerson.DisplayName;
            }
            else if (BunqContext.UserContext.isOnlyUserCompanySet())
            {
                return BunqContext.UserContext.UserCompany.DisplayName;
            }
            else
            {
                throw new BunqException("Unknown SDK user type.");
            }
        }

        public List<MonetaryAccountBank> GetAllMonetaryAccountBankActive(int count = 10)
        {
            var pagination = new Pagination {Count = count};

            var allMonetaryAccountBank = MonetaryAccountBank.List(pagination.UrlParamsCountOnly).Value;
            var allMonetaryAccountBankActive = new List<MonetaryAccountBank>();

            foreach (var monetaryAccountBank in allMonetaryAccountBank)
            {
                if (monetaryAccountBank.Status.Equals("ACTIVE"))
                {
                    allMonetaryAccountBankActive.Add(monetaryAccountBank);
                }
            }

            return allMonetaryAccountBankActive;
        }

        public static Pointer GetPointerIbanFromMonetaryAccountBank(MonetaryAccountBank monetaryAccountBank)
        {
            foreach (var alias in monetaryAccountBank.Alias)
            {
                if (alias.Type.Equals("IBAN"))
                {
                    return alias;
                }
            }

            throw new BunqException(
                string.Format(
                    "Could not find IBAN alias linked to MonetaryAccountBank \"%s\"",
                    monetaryAccountBank.Description
                )
            );
        }

        public List<Payment> GetAllPayment(int count = 10)
        {
            var pagination = new Pagination {Count = count};

            return Payment.List(urlParams: pagination.UrlParamsCountOnly).Value;
        }

        public List<RequestInquiry> GetAllRequest(int count = 10)
        {
            var pagination = new Pagination {Count = count};

            return RequestInquiry.List(urlParams: pagination.UrlParamsCountOnly).Value;
        }

        public List<Card> GetAllCard(int count = 10)
        {
            var pagination = new Pagination {Count = count};

            return Card.List(pagination.UrlParamsCountOnly).Value;
        }

        public static MonetaryAccountBank GetMonetaryAccountCurrentFromCard(Card card,
            List<MonetaryAccountBank> allMonetaryAccountBank)
        {
            var labelIban = card.LabelMonetaryAccountCurrent.LabelMonetaryAccount.Iban;

            foreach (var monetaryAccountBank in allMonetaryAccountBank)
            {
                if (labelIban.Equals(GetPointerIbanFromMonetaryAccountBank(monetaryAccountBank).Value))
                {
                    return monetaryAccountBank;
                }
            }

            return null;
        }

        public List<Pointer> GetAllUserAlias()
        {
            if (BunqContext.UserContext.isOnlyUserCompanySet())
            {
                return BunqContext.UserContext.UserCompany.Alias;
            }
            else if (BunqContext.UserContext.IsOnlyUserPersonSet())
            {
                return BunqContext.UserContext.UserPerson.Alias;
            }
            else
            {
                throw new BunqException("Could not determine user");
            }
        }

        private SandboxUser GenerateNewSandboxUser()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Client-Request-Id", "unique");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Geolocation", "0 0 0 0 NL");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Language", "en_US");
            httpClient.DefaultRequestHeaders.Add("X-Bunq-Region", "en_US");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "hoi");
            
            var requestTask = httpClient.PostAsync("https://sandbox.public.api.bunq.com/v1/sandbox-user", null);
            requestTask.Wait();
            
            var responseString = requestTask.Result.Content.ReadAsStringAsync().Result;
            var responseJson = BunqJsonConvert.DeserializeObject<JObject>(responseString);
            return BunqJsonConvert.DeserializeObject<SandboxUser>(responseJson.First.First.First.First.First.ToString());
        }
    }
}