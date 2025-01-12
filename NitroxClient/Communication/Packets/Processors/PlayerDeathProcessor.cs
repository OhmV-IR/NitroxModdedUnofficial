using System.Collections;
using System.Collections.Generic;
using NitroxClient.Communication.Packets.Processors.Abstract;
using NitroxClient.GameLogic;
using NitroxClient.GameLogic.Spawning.WorldEntities;
using NitroxClient.MonoBehaviours;
using NitroxModel.DataStructures.Unity;
using NitroxModel.Helper;
using NitroxModel.Packets;
using NitroxModel_Subnautica.DataStructures;
using UnityEngine;

namespace NitroxClient.Communication.Packets.Processors;

public class PlayerDeathProcessor : ClientPacketProcessor<PlayerDeathEvent>
{
    private readonly PlayerManager playerManager;
    private readonly LocalPlayer localPlayer;

    public PlayerDeathProcessor(PlayerManager playerManager, LocalPlayer localPlayer)
    {
        this.playerManager = playerManager;
        this.localPlayer = localPlayer;
    }

    public override void Process(PlayerDeathEvent playerDeath)
    {
        RemotePlayer player = Validate.IsPresent(playerManager.Find(playerDeath.PlayerId));
        Log.Debug($"{player.PlayerName} died");
        Log.InGame(Language.main.Get("Nitrox_PlayerDied").Replace("{PLAYER}", player.PlayerName));
        Log.InGame($"player died at {playerDeath.DeathPosition.X}, {playerDeath.DeathPosition.Y}, {playerDeath.DeathPosition.Z}");
        player.PlayerDeathEvent.Trigger(player);
        // TODO: Add any death related triggers (i.e. scoreboard updates, rewards, etc.)
    }
}
public class DeathBeacon : MonoBehaviour
{
    private static List<GameObject> activeDeathBeacons = new();
    private static readonly float despawnDistance = 20f;
    public static IEnumerator SpawnDeathBeacon(NitroxVector3 location, string playerName)
    {
        TaskResult<GameObject> result = new TaskResult<GameObject>();
        yield return DefaultWorldEntitySpawner.RequestPrefab(TechType.Beacon, result);
        GameObject beaconPrefab = result.Get();
        if(beaconPrefab != null)
        {
            GameObject beacon = Instantiate(beaconPrefab);
            Pickupable item = beacon.GetComponent<Pickupable>();
            if (item != null)
            {
                using (PacketSuppressor<EntitySpawnedByClient>.Suppress())
                {
                    item.Drop(location.ToUnity(), new Vector3(0, 0, 0), false);
                }
                beacon.GetComponent<Beacon>().label = $"{playerName}'s death";
                beacon.AddComponent<DeathBeacon>();
                activeDeathBeacons.Add(beacon);
            }
            else
            {
                Log.Error("Something went wrong in using the pickupable!");
            }
        }
        else
        {
            Log.Error("Something went wrong in getting the prefab");
        }
        yield break;
    }
    private void Update()
    {
        if(Vector3.Distance(Player.main.transform.position, transform.position) <= despawnDistance)
        {
            Destroy(gameObject);
            activeDeathBeacons.Remove(gameObject);
        }
    }
    public static void DespawnAllDeathBeacons()
    {
        foreach (GameObject beacon in activeDeathBeacons)
        {
            Destroy(beacon);
        }
        activeDeathBeacons.Clear();
    }
}
