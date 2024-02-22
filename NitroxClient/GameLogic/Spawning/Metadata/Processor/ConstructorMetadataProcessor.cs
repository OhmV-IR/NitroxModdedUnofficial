using NitroxClient.GameLogic.Spawning.Metadata.Processor.Abstract;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using UnityEngine;
using static NitroxModel.DisplayStatusCodes;

namespace NitroxClient.GameLogic.Spawning.Metadata.Processor;

public class ConstructorMetadataProcessor : EntityMetadataProcessor<ConstructorMetadata>
{
    public override void ProcessMetadata(GameObject gameObject, ConstructorMetadata metadata)
    {
        Constructor constructor = gameObject.GetComponent<Constructor>();

        if (constructor)
        {
            constructor.Deploy(metadata.Deployed);
        }
        else
        {
            DisplayStatusCode(StatusCode.SUBNAUTICA_ERROR, false, $"Could not find constructor on {gameObject.name}");
        }
    }
}
