using System;
using System.Collections.Generic;
using Bunq.Sdk.Model.Core;
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

            var allNotificationFilter = NotificationFilterUrlUserInternal.List().Value;
            var allNotificationFilterUrl = new List<NotificationFilterUrl>();

            foreach (var notificationFilterUser in allNotificationFilter)
            {
                allNotificationFilterUrl.AddRange(notificationFilterUser.NotificationFilters);
            }

            allNotificationFilterUrl = UpdateAllNotificationFilter(allNotificationFilterUrl, callbackUrl);
            NotificationFilterUrlUserInternal.CreateWithListResponse(allNotificationFilterUrl);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Callback URL added");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Check your changed overview");
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            bunq.UpdateContext();
        }

        private List<NotificationFilterUrl> UpdateAllNotificationFilter(
            List<NotificationFilterUrl> allNotificationFilter,
            string callbackUrl
        )
        {
            List<NotificationFilterUrl> allNotificationFilterUpdated = new List<NotificationFilterUrl>();

            foreach (var notificationFilter in allNotificationFilter)
            {
                if (callbackUrl.Equals(notificationFilter.NotificationTarget) == false)
                {
                    allNotificationFilterUpdated.Add(notificationFilter);
                }
            }
            
            allNotificationFilterUpdated.Add(new NotificationFilterUrl("MUTATION", callbackUrl));

            return allNotificationFilterUpdated;
        }
    }
}