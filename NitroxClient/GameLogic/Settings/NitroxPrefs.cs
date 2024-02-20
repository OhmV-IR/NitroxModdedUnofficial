using System;
using UnityEngine;
using static NitroxModel.DisplayStatusCodes;
namespace NitroxClient.GameLogic.Settings
{
    public class NitroxPrefs
    {
        // Add new fields here, you can use bool, float and int as type
        public static readonly NitroxPref<bool> HideIp = new("Nitrox.hideIp");
        public static readonly NitroxPref<bool> SilenceChat = new("Nitrox.silenceChat");
        public static readonly NitroxPref<bool> ChatUsed = new("Nitrox.chatUsed");
        public static readonly NitroxPref<bool> SafeBuilding = new("Nitrox.safeBuilding", true);
        public static readonly NitroxPref<bool> SafeBuildingLog = new("Nitrox.safeBuildingLog", true);
    }

    public abstract class NitroxPref { }

    public class NitroxPref<T> : NitroxPref where T : IConvertible
    {
        public string Key { get; }
        public T DefaultValue { get; }

        public NitroxPref(string key, T defaultValue = default)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                switch (DefaultValue)
                {
                    case bool defaultBool:
                        return (T)Convert.ChangeType(PlayerPrefs.GetInt(Key, defaultBool ? 1 : 0), typeof(T));
                    case float defaultFloat:
                        return (T)Convert.ChangeType(PlayerPrefs.GetFloat(Key, defaultFloat), typeof(T));
                    case int defaultInt:
                        return (T)Convert.ChangeType(PlayerPrefs.GetInt(Key, defaultInt), typeof(T));
                    default:
                        DisplayStatusCode(StatusCode.invalidVariableVal, false);
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (value)
                {
                    case bool boolValue:
                        PlayerPrefs.SetInt(Key, boolValue ? 1 : 0);
                        break;
                    case float floatValue:
                        PlayerPrefs.SetFloat(Key, floatValue);
                        break;
                    case int intValue:
                        PlayerPrefs.SetInt(Key, intValue);
                        break;
                    default:
                        DisplayStatusCode(StatusCode.invalidVariableVal, false);
                        throw new ArgumentOutOfRangeException();
                }
                PlayerPrefs.Save();
            }
        }
    }
}
