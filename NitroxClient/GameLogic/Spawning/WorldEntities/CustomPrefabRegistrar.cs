using System;
using UnityEngine;
using NitroxClient.Unity.Helper;
using System.Collections;
using System.Collections.Generic;
using UWE;
namespace NitroxClient.GameLogic.Spawning.WorldEntities;

public partial class DefaultWorldEntitySpawner
{
    static DefaultWorldEntitySpawner()
    {
        Log.Info("default world entity spawner initialized");
        CoroutineHost.StartCoroutine(BuildPosterPrefab());
    }
    private static IEnumerator BuildPosterPrefab()
    {
        TaskResult<GameObject> result = new();
        yield return RequestPrefab(TechType.PosterAurora, result);
        GameObject prefab = result.Get();
        yield return AssetBundleLoader.LoadAllAssets(AssetBundleLoader.NitroxAssetBundle.CUSTOM_POSTER_TEXTURE);
        foreach (UnityEngine.Object asset in AssetBundleLoader.NitroxAssetBundle.CUSTOM_POSTER_TEXTURE.LoadedAssets)
        {
            if (asset is Texture texture)
            {
                if (asset.name.Equals("custompostertexture"))
                {
                    var mats = prefab.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().materials;
                    Shader shader = mats[1].shader;
                    for (int j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                            {
                                mats[1].SetTexture(shader.GetPropertyName(j), texture);
                            }
                        }
                    prefab.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().materials = mats;
                    break;
                }
                }
            }
        prefab.name = "Nitrox poster";
        RegisterCustomPrefab((TechType)2147483547, prefab);
        RegisterCustomPrefab("916cbea4-b4bf-4311-8264-428bfef2241c", prefab);
    }

    public static void RegisterCustomPrefab(TechType techType, GameObject customPrefab)
    {
        if (prefabCacheByTechType.ContainsKey(techType))
        {
            throw new ArgumentException($"Can't register custom prefab {customPrefab.name} because another one is already registered with TechType {techType}.");
        }
        prefabCacheByTechType.Add(techType, customPrefab);
        Log.Info($"Successfully registered custom prefab {customPrefab.name} with TechType: {techType}");
    }

    public static void RegisterCustomPrefab(string classId, GameObject customPrefab)
    {
        if (prefabCacheByClassId.ContainsKey(classId))
        {
            throw new ArgumentException($"Can't register custom prefab {customPrefab.name} because another one is already registered with class id {classId}.");
        }
        prefabCacheByClassId.Add(classId, customPrefab);
        Log.Info($"Successfully registered custom prefab {customPrefab.name} with class id: {classId}");
    }
}
public class UpdatePoster : MonoBehaviour
{
    public Texture texture;
    public int jCap;
    public int iCap;
    public void UpdatePosterWithTexture()
    {
        var mats = gameObject.GetComponent<MeshRenderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            Shader shader = mats[i].shader;
            Log.Info("Shader length below");
            Log.Info(shader.GetPropertyCount());

            for (int j = 0; j < shader.GetPropertyCount(); j++)
            {
                if (shader.GetPropertyType(j) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                {
                    if (j > jCap) { }
                    else
                    {
                        mats[i].SetTexture(shader.GetPropertyName(j), texture);
                    }
                }
            }
        }
        gameObject.GetComponent<MeshRenderer>().materials = mats;
        Log.Info("Materials length below");
        Log.Info(gameObject.GetComponent<MeshRenderer>().materials.Length);
    }
}
