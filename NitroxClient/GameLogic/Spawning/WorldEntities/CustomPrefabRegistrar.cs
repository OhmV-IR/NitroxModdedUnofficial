using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace NitroxClient.GameLogic.Spawning.WorldEntities;
public partial class DefaultWorldEntitySpawner : IWorldEntitySpawner, IWorldEntitySyncSpawner
{
    static DefaultWorldEntitySpawner()
    {
        BuildPosterPrefab();
    }

    private static void BuildPosterPrefab()
    {
        // REMOVE THIS COMMENT: Do your stuff creating the poster here, you can also import the prefab thanks from an asset bundle
        GameObject prefab = new();

        // REMOVE THIS CONTENT: Make sure to use the same TechType here as in BatchSpawner(or smth).cs
        RegisterCustomPrefab((TechType)7575, prefab);
        RegisterCustomPrefab("916cbea4-b4bf-4311-8264-4228bfef2241c", prefab);
    }

    public static void RegisterCustomPrefab(TechType techType, GameObject customPrefab)
    {
        if (prefabCacheByTechType.ContainsKey(techType))
        {
            throw new ArgumentException($"Can't register custom prefab {customPrefab.name} because another one is already registered with TechType {techType}.");
        }
        prefabCacheByTechType.Add(techType, customPrefab);
        Log.Info(techType.ToString());
        Log.Info($"Successfully register custom prefab {customPrefab.name} with TechType: {techType}");
    }

    public static void RegisterCustomPrefab(string classId, GameObject customPrefab)
    {
        if (prefabCacheByClassId.ContainsKey(classId))
        {
            throw new ArgumentException($"Can't register custom prefab {customPrefab.name} because another one is already registered with class id {classId}.");
        }
        prefabCacheByClassId.Add(classId, customPrefab);
        Log.Info($"Successfully register custom prefab {customPrefab.name} with class id: {classId}");
    }
}
