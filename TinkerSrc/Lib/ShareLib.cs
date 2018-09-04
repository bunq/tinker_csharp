using System;
using System.Collections.Generic;
using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using Mono.Options;

namespace TinkerSrc.Lib
{
    public static class ShareLib
    {
        private const string OptionProduction = "production";
        private const string OptionAmount = "amount=";
        private const string OptionAccountId = "account-id=";
        private const string OptionDescription = "description=";
        private const string OptionName = "name=";
        private const string OptionCardId = "card-id=";
        private const string OptionCallbackUrl = "url=";
        private const string OptionRecipient = "recipient=";

        private const string PointerTypePhone = "PHONE_NUMBER";

        private static ApiEnvironmentType _environmentType = ApiEnvironmentType.SANDBOX;
        
        public static ApiEnvironmentType DetermineEnvironmentType(string[] args)
        {
            new OptionSet
            {
                {OptionProduction, v => _environmentType = ApiEnvironmentType.PRODUCTION}
            }.Parse(args);

            return _environmentType;
        }

        public static string GetAmountFromArgsOrStdIn(string[] args)
        {
            string amount = null;
            
            new OptionSet
            {
                {OptionAmount, v => amount = v }
            }.Parse(args);

            if (amount != null) return amount;
            
            Console.Write(Environment.NewLine + "    Amount (EUR): ");
            
            return Console.ReadLine();
        }

        public static string GetAccountIdFromArgsOrStdIn(string[] args)
        {
            string accountId = null;
            
            new OptionSet
            {
                {OptionAccountId, v => accountId = v }
            }.Parse(args);

            if (accountId != null) return accountId;
            
            Console.Write(Environment.NewLine + "    Account (ID):    ");
            
            return Console.ReadLine();
        }

        public static string GetDescriptionFromArgsOrStdIn(string[] args)
        {
            string description = null;
            
            new OptionSet
            {
                {OptionDescription, v => description = v}
            }.Parse(args);

            if (description != null) return description;

            Console.Write(Environment.NewLine + "    Description:  ");
            
            return Console.ReadLine();
        }

        public static string GetNameFromArgsOrStdIn(string[] args)
        {
            string name = null;
            
            new OptionSet
            {
                {OptionName, v => name = v}
            }.Parse(args);

            if (name != null) return name;

            Console.Write(Environment.NewLine + "    New Name:        ");
            
            return Console.ReadLine();
        }

        public static string GetCardIdFromArgsOrStdIn(string[] args)
        {
            string cardId = null;
            
            new OptionSet
            {
                {OptionCardId, v => cardId = v}
            }.Parse(args);

            if (cardId != null) return cardId;
         
            Console.Write(Environment.NewLine + "    Card (ID):       ");
            
            return Console.ReadLine();
        }

        public static string GetCallbackUrlFromArgsOrStdIn(string[] args)
        {
            string callbackUrl = null;
            
            new OptionSet
            {
                {OptionCallbackUrl, v => callbackUrl = v}
            }.Parse(args);

            if (callbackUrl != null) return callbackUrl;
         
            Console.Write(Environment.NewLine + "    Callback URL:    ");
            
            return Console.ReadLine();
        }

        public static string GetRecipientFromArgsOrStdIn(string[] args)
        {
            string recipient = null;
            
            new OptionSet
            {
                {OptionRecipient, v => recipient = v}
            }.Parse(args);

            if (recipient != null) return recipient;
            
            var hint = ApiEnvironmentType.PRODUCTION.Equals(_environmentType) ? "EMAIL" : "e.g. bravo@bunq.com";
                
            Console.Write(Environment.NewLine + $"    Recipient ({hint}): ");
                
            return Console.ReadLine();
        }

        public static void PrintHeader()
        {
            if (ApiEnvironmentType.PRODUCTION.Equals(_environmentType))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Out.Write(@"
  ██████╗ ██████╗  ██████╗ ██████╗ ██╗   ██╗ ██████╗████████╗██╗ ██████╗ ███╗   ██╗
  ██╔══██╗██╔══██╗██╔═══██╗██╔══██╗██║   ██║██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║
  ██████╔╝██████╔╝██║   ██║██║  ██║██║   ██║██║        ██║   ██║██║   ██║██╔██╗ ██║
  ██╔═══╝ ██╔══██╗██║   ██║██║  ██║██║   ██║██║        ██║   ██║██║   ██║██║╚██╗██║
  ██║     ██║  ██║╚██████╔╝██████╔╝╚██████╔╝╚██████╗   ██║   ██║╚██████╔╝██║ ╚████║
  ╚═╝     ╚═╝  ╚═╝ ╚═════╝ ╚═════╝  ╚═════╝  ╚═════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Out.Write(@"
  ████████╗██╗███╗   ██╗██╗  ██╗███████╗██████╗ ██╗███╗   ██╗ ██████╗ 
  ╚══██╔══╝██║████╗  ██║██║ ██╔╝██╔════╝██╔══██╗██║████╗  ██║██╔════╝ 
     ██║   ██║██╔██╗ ██║█████╔╝ █████╗  ██████╔╝██║██╔██╗ ██║██║  ███╗
     ██║   ██║██║╚██╗██║██╔═██╗ ██╔══╝  ██╔══██╗██║██║╚██╗██║██║   ██║
     ██║   ██║██║ ╚████║██║  ██╗███████╗██║  ██║██║██║ ╚████║╚██████╔╝
     ╚═╝   ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝ 
");
            }
            
            Console.ResetColor();
        }

        public static void PrintUser(int id, string displayName)
        {
            Console.Out.Write(Environment.NewLine + "   User");
            
            Console.Out.Write(@"
  ┌───────────────────┬────────────────────────────────────────────────────
  │ ID                │ " + id + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Username          │ " + displayName + @"
  └───────────────────┴────────────────────────────────────────────────────
");
        }

        public static void PrintAllMonetaryAccountBank(List<MonetaryAccountBank> allMonetaryAccountBankActive)
        {
            Console.Out.Write(Environment.NewLine + "   Monetary Accounts");

            foreach (var monetaryAccountBank in allMonetaryAccountBankActive)
            {
                PrintMonetaryAccountBank(monetaryAccountBank);
            }
        }

        private static void PrintMonetaryAccountBank(MonetaryAccountBank monetaryAccountBank)
        {
            var pointerIban = BunqLib.GetPointerIbanFromMonetaryAccountBank(monetaryAccountBank);

            Console.Out.Write(@"
  ┌───────────────────┬────────────────────────────────────────────────────
  │ ID                │ " + monetaryAccountBank.Id + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Description       │ " + monetaryAccountBank.Description + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ IBAN              │ " + pointerIban.Value);

            if (monetaryAccountBank.Balance == null)
            {
                // Cannot show balance, as we dont have permission to view it.
            }
            else
            {
                Console.Out.Write(@"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Balance           │ " + monetaryAccountBank.Balance.Currency + " " + monetaryAccountBank.Balance.Value);
            }
           Console.Out.Write(@"
  └───────────────────┴────────────────────────────────────────────────────
");
        }

        public static void PrintAllPayment(List<Payment> allPayment)
        {
            Console.Out.Write(Environment.NewLine + "   Payments");

            foreach (var payment in allPayment)
            {
                PrintPayment(payment);
            }
        }

        private static void PrintPayment(Payment payment)
        {
            Console.Out.Write(@"
  ┌───────────────────┬────────────────────────────────────────────────────
  │ ID                │ " + payment.Id + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Description       │ " + payment.Description + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Amount            │ " + payment.Amount.Currency + " " + payment.Amount.Value + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Recipient         │ " + payment.CounterpartyAlias.LabelMonetaryAccount.DisplayName + @"
  └───────────────────┴────────────────────────────────────────────────────
");
        }

        public static void PrintAllRequest(List<RequestInquiry> allRequest)
        {
            Console.Out.Write(Environment.NewLine + "   Requests");

            foreach (var request in allRequest)
            {
                PrintRequest(request);
            }
        }

        public static void PrintRequest(RequestInquiry request)
        {
            Console.Out.Write(@"
  ┌───────────────────┬────────────────────────────────────────────────────
  │ ID                │ " + request.Id + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Description       │ " + request.Description + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Status            │ " + request.Status + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Amount            │ " + request.AmountInquired.Currency + " " + request.AmountInquired.Value + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Recipient         │ " + request.CounterpartyAlias.LabelMonetaryAccount.DisplayName + @"
  └───────────────────┴────────────────────────────────────────────────────
");
        }

        public static void PrintAllCard(List<Card> allCard, List<MonetaryAccountBank> allMonetaryAccountBank)
        {
            Console.Out.Write(Environment.NewLine + "   Cards");

            foreach (var card in allCard)
            {
                PrintCard(card, allMonetaryAccountBank);
            }
        }

        private static void PrintCard(Card card, List<MonetaryAccountBank> allMonetaryAccountBank)
        {
            var monetaryAccountBank = BunqLib.GetMonetaryAccountCurrentFromCard(card, allMonetaryAccountBank);
            var cardDescription = card.SecondLine ?? "bunq card";
            var monetaryAccountDescription = monetaryAccountBank.Description ?? "account description";

            Console.Out.Write(@"
  ┌───────────────────┬────────────────────────────────────────────────────
  │ ID                │ " + card.Id + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Type              │ " + card.Type + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Name on Card      │ " + card.NameOnCard + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Description       │ " + cardDescription + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Linked Account    │ " + monetaryAccountDescription + " " + card.LabelMonetaryAccountCurrent.LabelMonetaryAccount.Iban + @"
  └───────────────────┴────────────────────────────────────────────────────");
        }

        public static void PrintAllUserAlias(IEnumerable<Pointer> allUserAlias)
        {
            Console.Out.Write("\n   You can use these login credentials to login in to the bunq sandbox app.");

            foreach (var alias in allUserAlias)
            {
                Console.Out.Write(@"
  ┌───────────────────┬────────────────────────────────────────────────────
  │ Value             │ " + alias.Value + @"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Type              │ " + alias.Type
                                  );
                if (alias.Type.Equals(PointerTypePhone)) {
                    Console.Out.Write(@"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Confirmation code │ 123456"
                                      );
                }
                Console.Out.Write(@"
  ├───────────────────┼────────────────────────────────────────────────────
  │ Login code        │ 000000
  └───────────────────┴────────────────────────────────────────────────────"
                                  );
            }
        }
    }
}
