﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class used as a skill identifier
//you can use skill's position in the skill tree or its name
[System.Serializable]
public class SkillID
{
    public string skillName = string.Empty;

    public byte
        treeID,
        branchID,
        elementID;


    public bool UsesString()
    {
        return skillName.Length > 0;
    }
}
