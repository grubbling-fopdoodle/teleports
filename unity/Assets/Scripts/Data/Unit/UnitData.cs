﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

public partial class UnitData {
    
    /*private const int labelWidth = 110;

    [SerializeField, PropertyOrder(-5), LabelWidth(labelWidth)] private string unitName;
    [SerializeField, LabelWidth(labelWidth)] private string raceName;
    [SerializeField, PropertyOrder(-4), LabelWidth(labelWidth)] private int level;
    [SerializeField, InlineProperty, LabelWidth(labelWidth)] private UnitAbilities abilities;    
    [SerializeField, InlineProperty, LabelWidth(labelWidth)] private UnitAttributes attributes;
    [SerializeField, InlineProperty, LabelWidth(labelWidth)] private SkillID mainAttack;
    [SerializeField, LabelWidth(labelWidth)] private List<PerkID> perks;
    [SerializeField, LabelWidth(labelWidth)] private List<SkillID> skills;
    [SerializeField, LabelWidth(labelWidth)] private InventoryData inventory;
    [SerializeField, HideInInspector] private bool isInitialized;*/

    public UnitData()
    {
        isInitialized = false;
        Initialize();
    }

    [Button, HideIf("IsInitialized")]
    public void Initialize()
    {
        unitName = DataDefaults.unitName;
        raceName = DataDefaults.raceName;
        level = 1;
        attributes = new UnitAttributesData();
        mainAttack = new SkillID();
        perks = new List<PerkID>();
        skills = new List<SkillID>();
        inventory = new InventoryData();
        isInitialized = true;
    }

    [Button]
    public void CorrectInvalidData()
    {
        if(string.IsNullOrEmpty(unitName))
        {
            Debug.LogWarning("Unit name not found, changing to " + DataDefaults.unitName);
            unitName = DataDefaults.unitName;
        }
        if(raceName == null || !Main.StaticData.Game.Races.ContainsName(raceName))
        {
            Debug.LogWarning("Invalid race name, changing to " + DataDefaults.raceName);
            raceName = DataDefaults.raceName;
        }
        if(mainAttack == null || !Main.StaticData.Game.Skills.ContainsName(mainAttack.Name))
        {
            Debug.LogWarning("Invalid main attack, changing to " + DataDefaults.skillName);
            mainAttack = new SkillID();
        }
        Inventory.CorrectInvalidData();

        Attributes.CorrectInvalidData();
    }

    public Attribute GetAttribute(UnitAttributesData.AttributeType type) {
        return attributes.GetAttribute(type);
    }

    public SkillID CurrentWeaponMainSkillId
    {
        get
        {
            foreach (var itemInfo in Inventory.EquipmentData.GetEquippedItems())
            {
                if (itemInfo.Item.IsType(ItemType.Weapon))
                {
                    return itemInfo.Item.Skills.Count > 0 ? itemInfo.Item.Skills?[0] : null;
                }
            }
            return null;
        }
    }

    public string Name
    {
        get { return unitName; }
        set { unitName = value; }
    }
    public float Height => GetAttribute(UnitAttributesData.AttributeType.Height).Value;
    public List<MappedListID> SkillIds { get { return skills.ConvertAll(x => (MappedListID)x); } }
    public List<MappedListID> PerkIds { get { return perks.ConvertAll(x => (MappedListID)x); } }
}
