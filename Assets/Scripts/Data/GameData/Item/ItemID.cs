﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[System.Serializable]
public class ItemID : MappedListID {

    protected override IList<string> DropdownValues()
    {
        return Main.StaticData.Game.Items.AllNames;
    }
}
