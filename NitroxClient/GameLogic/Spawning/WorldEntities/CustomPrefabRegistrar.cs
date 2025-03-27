using System;
using System.Collections;
using NitroxClient.Unity.Helper;
using UnityEngine;
using UWE;
namespace NitroxClient.GameLogic.Spawning.WorldEntities;

public partial class DefaultWorldEntitySpawner
{
    static DefaultWorldEntitySpawner()
    {
        CoroutineHost.StartCoroutine(BuildTeamPosterPrefab());
    }
    private static IEnumerator BuildTeamPosterPrefab()
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
                    Material[] mats = prefab.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().materials;
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
        prefab.AddComponent<UpdatePosterLocation>();
        prefab.GetComponent<UpdatePosterLocation>().posterLocation = new Vector3(972f, 11.75f, -74.6f);
        prefab.GetComponent<UpdatePosterLocation>().posterRotation = Quaternion.Euler(new Vector3(0, 46, 0));
        prefab.name = "Nitrox team poster";
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
public class UpdatePosterLocation : MonoBehaviour
{
    public Vector3 posterLocation;
    public Quaternion posterRotation;
    public void Start()
    {
        if (posterLocation != null)
        {
            gameObject.transform.position = posterLocation;
        }
        if (posterRotation != null) {
            gameObject.transform.rotation = posterRotation;
        }
    }
}
