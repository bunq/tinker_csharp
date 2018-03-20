using System;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class MakeRequest : ITinker
    {
        public void Run(string[] args)
        {
            var environmentType = ShareLib.DetermineEnvironmentType(args);
            
            ShareLib.PrintHeader();
            
            var bunq = new BunqLib(environmentType);

            var amount = ShareLib.GetAmountFromArgsOrStdIn(args);
            var recipient = ShareLib.GetRecipientFromArgsOrStdIn(args);
            var description = ShareLib.GetDescriptionFromArgsOrStdIn(args);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine($"  | Requesting:   € {amount}");
            Console.Out.WriteLine($"  | From:         {recipient}");
            Console.Out.WriteLine($"  | Description:  {description}");
            Console.Out.WriteLine();
            Console.Out.WriteLine("    ...");
            Console.Out.WriteLine();

            RequestInquiry.Create(new Amount(amount, "EUR"), new Pointer("EMAIL", recipient), description, true);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Request sent");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Check your changed overview");
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            bunq.UpdateContext();
        }
    }
}