using System;
using Bunq.Sdk.Model.Generated.Endpoint;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class UpdateAccount : ITinker
    {
        public void Run(string[] args)
        {
            var environmentType = ShareLib.DetermineEnvironmentType(args);
            
            ShareLib.PrintHeader();
            
            var bunq = new BunqLib(environmentType);

            var accountId = ShareLib.GetAccountIdFromArgsOrStdIn(args);
            var name = ShareLib.GetNameFromArgsOrStdIn(args);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine($"  | Updating Name:      {name}");
            Console.Out.WriteLine($"  | of Account:         {accountId}");
            Console.Out.WriteLine();
            Console.Out.WriteLine("    ...");
            Console.Out.WriteLine();

            MonetaryAccountBank.Update(int.Parse(accountId), name);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Account updated");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Check your changed overview");
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            bunq.UpdateContext();
        }
    }
}