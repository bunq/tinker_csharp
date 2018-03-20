using System;
using Bunq.Sdk.Model.Generated.Endpoint;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class LinkCard : ITinker
    {
        public void Run(string[] args)
        {
            var environmentType = ShareLib.DetermineEnvironmentType(args);
            
            ShareLib.PrintHeader();
            
            var bunq = new BunqLib(environmentType);

            var cardId = ShareLib.GetCardIdFromArgsOrStdIn(args);
            var accountId = ShareLib.GetAccountIdFromArgsOrStdIn(args);
            
            Console.Out.WriteLine();
            Console.Out.WriteLine($"  | Link Card:    {cardId}");
            Console.Out.WriteLine($"  | To Account:   {accountId}");
            Console.Out.WriteLine();
            Console.Out.WriteLine("    ...");
            Console.Out.WriteLine();

            Card.Update(int.Parse(cardId), monetaryAccountCurrentId: int.Parse(accountId));
            
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Account switched");
            Console.Out.WriteLine();
            Console.Out.WriteLine("  | Check your changed overview");
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            bunq.UpdateContext();
        }
    }
}