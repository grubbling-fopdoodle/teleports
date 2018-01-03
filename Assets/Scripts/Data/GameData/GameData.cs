﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "gameData", menuName = "Data/Game")]
[ShowOdinSerializedPropertiesInInspector]
public class GameData : SerializedScriptableObject {

    [SerializeField] private MappedList<Gem> gems;
    [SerializeField] private MappedList<WorldData> worlds;
    [SerializeField] private MappedList<Race> races;
    [SerializeField] private MappedList<Perk> perks;
    [SerializeField] private MappedList<Skill> skills;
    [SerializeField] private MappedList<ItemAssetData> items;
    [SerializeField] private SkillDatabase skillDatabase;
    [SerializeField] private GraphicsData graphicsData;

    public void OnEnable()
    {
        gems.MakeDict();
        worlds.MakeDict();
        races.MakeDict();
        perks.MakeDict();
        skills.MakeDict();
        items.MakeDict();
    }

    public ItemData GetItem(string itemName)
    {
        ItemAssetData result = items.TryGetValue(itemName);

        if (result != null)
        {
            return result.GenerateItem();
        }
        else
        {
            Debug.Log("Item " + itemName + " not found.");
            return null;
        }
    }
    
    public Race GetRace(string raceName)
    {
        Race result = races.TryGetValue(raceName);

        if (result != null)
        {
            return result;
        }
        else
        {
            Debug.Log("Race " + raceName + " not found.");
            return null;
        }
    }

    public List<Race> GetPlayableRaces()
    {
        var result = new List<Race>();

        foreach (Race race in races.AllValues)
        {
            if (race.IsPlayable)
            {
                result.Add(race);
            }
        }

        return result;
    }

    public Skill GetSkill(SkillID skillId)
    {
        if (skillId.UsesString())
        {
            return skills.TryGetValue(skillId.skillName);
        }
        else
        {
            return skillDatabase.GetSkill(skillId);
        }
    }

    public SkillDatabase CurrentSkillDatabase
    {
        get { return skillDatabase; }
    }

    public GraphicsData GraphicsData
    {
        get { return graphicsData; }
    }

    public IList<string> PerkNames
    {
        get { return perks.AllNames; }
    }

    public IList<string> ItemNames
    {
        get { return items.AllNames; }
    }
}
