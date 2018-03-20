using System;
using System.Collections.Generic;
using Bunq.Sdk.Context;
using Bunq.Sdk.Exception;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class AddCallbackUrl : ITinker
    {
        public void Run(string[] args)
        {
            var environmentType = ShareLib.DetermineEnvironmentType(args);
            
            ShareLib.PrintHeader();
            
            var bunq = new BunqLib(environmentType);

            var callbackUrl = ShareLib.GetCallbackUrlFromArgsOrStdIn(args);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine($"  | Adding Callback URL:    {callbackUrl}");
            Console.Out.WriteLine();
            Console.Out.WriteLine("    ...");
            Console.Out.WriteLine();

            if (BunqContext.UserContext.isOnlyUserCompanySet())
            {
                UserCompany.Update(
                    notificationFilters: UpdateAllNotificationFilter(
                        BunqContext.UserContext.UserCompany.NotificationFilters,
                        callbackUrl
                    )
                );
            }
            else if (BunqContext.UserContext.IsOnlyUserPersonSet())
            {
                UserPerson.Update(
                    notificationFilters: UpdateAllNotificationFilter(
                        BunqContext.UserContext.UserPerson.NotificationFilters,
                        callbackUrl
                    )
                );
            }
            else
            {
                throw new BunqException("Unexpected user type found.");
            }
            
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Callback URL added");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Check your changed overview");
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            bunq.UpdateContext();
        }

        private List<NotificationFilter> UpdateAllNotificationFilter(
            List<NotificationFilter> allNotificationFilter,
            string callbackUrl
        )
        {
            List<NotificationFilter> allNotificationFilterUpdated = new List<NotificationFilter>();

            foreach (var notificationFilter in allNotificationFilter)
            {
                if (callbackUrl.Equals(notificationFilter.NotificationTarget) == false)
                {
                    allNotificationFilterUpdated.Add(notificationFilter);
                }
            }
            
            allNotificationFilterUpdated.Add(new NotificationFilter("URL", callbackUrl, "MUTATION"));

            return allNotificationFilterUpdated;
        }
    }
}