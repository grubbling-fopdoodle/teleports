﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEditor;

public class AssetInfo <T> where T : Object
{

    private T assetObject;
    private string guid;
    private string path;
    private string name;

    public AssetInfo(string guid)
    {
        this.guid = guid;
        path = AssetDatabase.GUIDToAssetPath(guid);
        assetObject = AssetDatabase.LoadAssetAtPath<T>(path);
        name = System.IO.Path.GetFileNameWithoutExtension(path);
    }

    public void SetUniqueNameToFilename()
    {
        SerializedObject so = new SerializedObject(assetObject);
        SerializedProperty sp = so.FindProperty("uniqueName");
        if (sp == null)
        {
            Debug.LogWarning($"uniqueName not found in {path}");
            return;
        }

        if (!sp.stringValue.IsNullOrWhitespace())
        {
            Debug.LogWarning($"uniqueName of {path} already set to {sp.stringValue}, not overriding");
            return;
        }
        sp.stringValue = name;
        Debug.Log($"Setting uniqueName of {path} to {name}");
        so.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    public T AssetObject => assetObject;
    public string Path => path;
    public string Name => name;
}
