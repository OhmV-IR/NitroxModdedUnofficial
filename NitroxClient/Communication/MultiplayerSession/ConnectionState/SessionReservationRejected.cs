using System;
using NitroxClient.Communication.Abstract;
using static NitroxModel.DisplayStatusCodes;
namespace NitroxClient.Communication.MultiplayerSession.ConnectionState
{
    public class SessionReservationRejected : ConnectionNegotiatedState
    {
        public override MultiplayerSessionConnectionStage CurrentStage => MultiplayerSessionConnectionStage.SESSION_RESERVATION_REJECTED;

        public override void JoinSession(IMultiplayerSessionConnectionContext sessionConnectionContext)
        {
            DisplayStatusCode(StatusCode.twenty);
            throw new InvalidOperationException("The session has rejected the reserveration request.");
        }
    }
}
