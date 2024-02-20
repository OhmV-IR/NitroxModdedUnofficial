using System.Collections.Generic;
using System.Net;
using NitroxModel.Platforms.OS.Shared;
using static NitroxServer.Server;
using static NitroxModel.DisplayStatusCodes;
namespace NitroxModel.Platforms.OS.MacOS
{
    public sealed class MacFileSystem : FileSystem
    {
        private bool throwNotImplementedException()
        {
            DisplayStatusCode(StatusCode.missingFeature, true, "Tried to access a feature that does not exist yet");
            return false;
        }
        public override IEnumerable<string> GetDefaultPrograms(string file)
        {
            yield return "open";
        }

        public override bool SetFullAccessToCurrentUser(string directory)
        {
            DisplayStatusCode(StatusCode.missingFeature, true, "Tried to access a feature that does not exist yet");
            return false;
        }

        public override bool IsTrustedFile(string file) => throwNotImplementedException();
    }
}
