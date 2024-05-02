using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using BinaryPack.Attributes;

namespace NitroxModel.DataStructures.GameLogic;

/// <summary>
///     TechType is the enum used in Subnautica for defining all the spawnable objects in the world. This includes food, enemies and bases.
/// </summary>
/// <remarks>
///     Shim tech type model to bridge the gap between original subnautica and BZ.
/// </remarks>
[Serializable]
[DataContract]
public class NitroxTechType : IEquatable<NitroxTechType>
{
    [DataMember(Order = 1)]
    public string Name { get; }

    [IgnoreConstructor]
    protected NitroxTechType()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }


    public NitroxTechType(string name)
    {
        Name = name;
    }

    public static NitroxTechType None { get; } = new NitroxTechType("None");

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((NitroxTechType)obj);
    }

    public bool Equals(NitroxTechType other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    }
    public bool TryParseCustom(out int enumValue)
    {
        return customTechTypes.TryGetValue(Name, out enumValue);
    }

    private static readonly Dictionary<string, int> customTechTypes = [];

    static NitroxTechType()
    {
        foreach (FieldInfo field in typeof(CustomTechType).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            customTechTypes.Add(field.Name, (int)field.GetRawConstantValue());
        }
    }

    public enum CustomTechType : int
    {
        CUSTOM = 2147483547
    }
}
