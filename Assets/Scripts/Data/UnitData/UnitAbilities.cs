﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnitAbility = System.Int32;

[System.Serializable]
public class UnitAbilities{

    public enum Type
    {
        STR,
        DEX,
        INT
    }

    [SerializeField, GUIColor(1, 0.5f, 0.5f)] private UnitAbility strength;
    [SerializeField, GUIColor(0.5f, 1, 0.5f)] private UnitAbility dexterity;
    [SerializeField, GUIColor(0.5f, 0.5f, 1)] private UnitAbility intelligence; 

    public UnitAbilities()
    {
        strength = 10;
        dexterity = 10;
        intelligence = 10;
    }

    public UnitAbilities(UnitAbilities other)
    {
        strength = other.strength;
        dexterity = other.dexterity;
        intelligence = other.intelligence;
    }

    public UnitAbility GetAbility(Type type)
    {
        switch (type)
        {
            case Type.STR:
                return strength;
            case Type.DEX:
                return dexterity;
            case Type.INT:
                return intelligence;
            default:
                return new UnitAbility();
        }
    }

    public UnitAbility Strength => strength;
    public UnitAbility Dexterity => dexterity;
    public UnitAbility Intelligence => intelligence;
}
