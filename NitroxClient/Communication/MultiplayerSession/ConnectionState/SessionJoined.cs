using System;
using NitroxClient.Communication.Abstract;
using static NitroxModel.DisplayStatusCodes;
namespace NitroxClient.Communication.MultiplayerSession.ConnectionState
{
    public class SessionJoined : ConnectionNegotiatedState
    {
        public override MultiplayerSessionConnectionStage CurrentStage => MultiplayerSessionConnectionStage.SESSION_JOINED;

        public override void JoinSession(IMultiplayerSessionConnectionContext sessionConnectionContext)
        {
            DisplayStatusCode(StatusCode.outboundConnectionAlreadyOpen);
            throw new InvalidOperationException("The session is already in progress.");
        }
    }
}
