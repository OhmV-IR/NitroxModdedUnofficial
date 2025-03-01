using NitroxClient.Communication;
using NitroxClient.Communication.Abstract;
using NitroxClient.GameLogic.Spawning.Metadata.Processor.Abstract;
using NitroxClient.Unity.Helper;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using NitroxModel.Packets;
using UnityEngine;

namespace NitroxClient.GameLogic.Spawning.Metadata.Processor;

public class CyclopsMetadataProcessor : EntityMetadataProcessor<CyclopsMetadata>
{
    private readonly IPacketSender packetSender;
    private readonly LiveMixinManager liveMixinManager;

    public CyclopsMetadataProcessor(IPacketSender packetSender, LiveMixinManager liveMixinManager)
    {
        this.packetSender = packetSender;
        this.liveMixinManager = liveMixinManager;
    }

    public override void ProcessMetadata(GameObject cyclops, CyclopsMetadata metadata)
    {
        using (PacketSuppressor<EntityMetadataUpdate>.Suppress())
        {
            SetEngineMode(cyclops, (CyclopsMotorMode.CyclopsMotorModes)metadata.EngineMode);
            ChangeSilentRunning(cyclops, metadata.SilentRunningOn);
            ChangeShieldMode(cyclops, metadata.ShieldOn);
            ChangeSonarMode(cyclops, metadata.SonarOn);
            SetEngineState(cyclops, metadata.EngineOn);
            SetHealth(cyclops, metadata.Health);
            SetDestroyed(cyclops, metadata.IsDestroyed);
        }
    }

    private void SetEngineState(GameObject cyclops, bool isOn)
    {
        CyclopsEngineChangeState engineState = cyclops.RequireComponentInChildren<CyclopsEngineChangeState>(true);

        if (isOn == engineState.motorMode.engineOn)
        {
            // engine state is the same - nothing to do.
            return;
        }

        // During initial sync or when the cyclops HUD isn't shown (from outside of the cyclops)
        if (Player.main.currentSub != engineState.subRoot)
        {
            engineState.startEngine = isOn;
            engineState.subRoot.BroadcastMessage(nameof(CyclopsMotorMode.InvokeChangeEngineState), isOn, SendMessageOptions.RequireReceiver);
            engineState.invalidButton = true;
            engineState.Invoke(nameof(CyclopsEngineChangeState.ResetInvalidButton), 2.5f);
        }
        // When inside of the cyclops, we play the cinematics
        else
        {
            // To invoke the whole OnClick method we need to set the right parameters first
            engineState.invalidButton = false;
            using (PacketSuppressor<EntityMetadataUpdate>.Suppress())
            {
                engineState.OnClick();
            }
        }
    }

    private void SetEngineMode(GameObject cyclops, CyclopsMotorMode.CyclopsMotorModes mode)
    {
        foreach (CyclopsMotorModeButton button in cyclops.GetComponentsInChildren<CyclopsMotorModeButton>(true))
        {
            // At initial sync, this kind of processor is executed before the Start()
            if (!button.subRoot)
            {
                button.Start();
            }

            button.SetCyclopsMotorMode(mode);
        }
    }

    private void ChangeSilentRunning(GameObject cyclops, bool isOn)
    {
        CyclopsSilentRunningAbilityButton ability = cyclops.RequireComponentInChildren<CyclopsSilentRunningAbilityButton>(true);

        if (isOn == ability.active)
        {
            return;
        }

        Log.Debug($"Set silent running to {isOn} for cyclops");
        ability.active = isOn;
        if (isOn)
        {
            ability.image.sprite = ability.activeSprite;
            ability.subRoot.BroadcastMessage("RigForSilentRunning");
            ability.InvokeRepeating(nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration), 0f, ability.silentRunningIteration);
        }
        else
        {
            ability.image.sprite = ability.inactiveSprite;
            ability.subRoot.BroadcastMessage("SecureFromSilentRunning");
            ability.CancelInvoke(nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration));
        }
    }

    private void ChangeShieldMode(GameObject cyclops, bool isOn)
    {
        CyclopsShieldButton shield = cyclops.GetComponentInChildren<CyclopsShieldButton>(true);

        if (!shield)
        {
            // may not have a shield installed.
            return;
        }

        bool isShieldOn = shield.activeSprite == shield.image.sprite;

        if (isShieldOn == isOn)
        {
            return;
        }

        if (isOn)
        {
            shield.StartShield();
        }
        else
        {
            shield.StopShield();
        }
    }

    private void ChangeSonarMode(GameObject cyclops, bool isOn)
    {
        CyclopsSonarButton sonarButton = cyclops.GetComponentInChildren<CyclopsSonarButton>(true);
        if (sonarButton && sonarButton.sonarActive != isOn)
        {
            if (isOn)
            {
                sonarButton.TurnOnSonar();
            }
            else
            {
                sonarButton.TurnOffSonar();
            }
        }
    }

    private void SetHealth(GameObject gameObject, float health)
    {
        LiveMixin liveMixin = gameObject.RequireComponentInChildren<LiveMixin>(true);
        liveMixinManager.SyncRemoteHealth(liveMixin, health);
    }

    private void SetDestroyed(GameObject gameObject, bool isDestroyed)
    {
        CyclopsDestructionEvent destructionEvent = gameObject.RequireComponentInChildren<CyclopsDestructionEvent>(true);

        // Don't play VFX and SFX if the Cyclops is already destroyed or was spawned in as destroyed
        if (destructionEvent.subRoot.subDestroyed == isDestroyed) return;

        if (isDestroyed)
        {
            // Use packet suppressor as sentinel so the patch callback knows not to spawn loot
            using (PacketSuppressor<EntitySpawnedByClient>.Suppress())
            {
                destructionEvent.DestroyCyclops();
            }
        }
        else
        {
            destructionEvent.RestoreCyclops();
        }
    }
}
