﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneTransitionHandler  {

    public static void HandleSceneTransition(string fromName, string toName)
    {
        if(fromName == SceneNames.Main && toName == SceneNames.Home)
        {
            //Main.LoadGameState();
        }
        else if (fromName == SceneNames.World && toName == SceneNames.Main)
        {
            Main.SaveGameState();
        }
    }
}
