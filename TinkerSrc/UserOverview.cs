using Bunq.Sdk.Context;
using Tinker.Utils;
using TinkerSrc.Lib;

namespace TinkerSrc
{
    public class UserOverview : ITinker
    {
        public void Run(string[] args)
        {
            var environmentType = ShareLib.DetermineEnvironmentType(args);
            
            ShareLib.PrintHeader();
            
            var bunq = new BunqLib(environmentType);

            ShareLib.PrintUser(BunqContext.UserContext.UserId, bunq.GetCurrentUserDisplayName());

            var allMonetaryAccountBankActive = bunq.GetAllMonetaryAccountBankActive(1);
            ShareLib.PrintAllMonetaryAccountBank(allMonetaryAccountBankActive);

            var allPayment = bunq.GetAllPayment(1);
            ShareLib.PrintAllPayment(allPayment);

            var allRequest = bunq.GetAllRequest(1);
            ShareLib.PrintAllRequest(allRequest);

            var allCard = bunq.GetAllCard(1);
            ShareLib.PrintAllCard(allCard, allMonetaryAccountBankActive);

            if (environmentType.Equals(ApiEnvironmentType.SANDBOX))
            {
                var allUserAlias = bunq.GetAllUserAlias();
                ShareLib.PrintAllUserAlias(allUserAlias);
            }

            bunq.UpdateContext();
        }
    }
}
