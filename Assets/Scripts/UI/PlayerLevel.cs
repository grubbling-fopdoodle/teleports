﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlayerLevel : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Text text = gameObject.GetComponent<Text>();
        text.text = "Level " + GlobalData.instance.playerData_.level().ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}