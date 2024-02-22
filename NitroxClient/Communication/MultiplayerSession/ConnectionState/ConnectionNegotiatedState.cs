using System;
using System.Threading.Tasks;
using NitroxClient.Communication.Abstract;
using static NitroxModel.DisplayStatusCodes;
namespace NitroxClient.Communication.MultiplayerSession.ConnectionState
{
    public abstract class ConnectionNegotiatedState : CommunicatingState
    {
        public override Task NegotiateReservationAsync(IMultiplayerSessionConnectionContext sessionConnectionContext)
        {
            DisplayStatusCode(StatusCode.CONNECTION_FAIL_CLIENT, false, "Unable to negotiate a session connection in the current state.");
            throw new Exception();
        }
    }
}
