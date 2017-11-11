﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
//singleton managing all the static data
[CreateAssetMenu(fileName = "mainData", menuName = "Data/Main")]
public partial class MainData : ScriptableObject {

    //singleton instance
    public static MainData instance;
    static bool isInitialized = false;

    //inspector fields
    [SerializeField]
    private SaveDataSO saveData;
    [SerializeField]
    private GameData gameData;
    [SerializeField]
    private ServerData serverData;
    [SerializeField]
    private Stylesheet stylesheet;

    public delegate void OnInitialized();
    public static event OnInitialized OnInitializedEvent;

    #region properties
    private static MainData Instance{
        get {
            if (instance == null)
            {
                instance = Resources.Load("Data/mainData") as MainData;
            }
            return instance;
        }
    }

    public static SaveData CurrentSaveData
    {
        get { return Instance.saveData.saveData; }
    }

    public static SaveDataSO SaveDataSO
    {
        get { return Instance.saveData; }
    }

    public static IPlayerData CurrentPlayerData
    {
        get { return CurrentSaveData.CurrentPlayerData(); }
    }

    public static GameData CurrentGameData
    {
        get { return Instance.gameData; }
    }

    public static IServerData CurrentServerData
    {
        get { return Instance.serverData; }
    }

    public static Stylesheet CurrentStylesheet
    {
        get { return Instance.stylesheet; }
    }

    public static string PlayerName
    {
        get { return CurrentPlayerData.CharacterName; }
    }

    public static int RankPoints
    {
        get { return CurrentPlayerData.RankPoints; }
    }

    public static int Xp
    {
        get { return CurrentPlayerData.Xp; }
    }
    #endregion

    //unity event functions
    void Awake()
    {
        Initialize();
    }

    void OnDestroy()
    {
        instance = null;
    }

    //editor only
    void OnEnable()
    {
        Initialize();
    }


    //public functions
    void Initialize()
    {
        if (!isInitialized || instance == null)
        {
            instance = this;
            isInitialized = true;
            CurrentSaveData.CorrectInvalidData();

            if (OnInitializedEvent != null)
            {
                OnInitializedEvent();
            }
            Debug.Log("MainData initialized!");
        }
    }

    public static void SavePlayer(GameObject player)
    {
        CurrentPlayerData.Xp = player.GetComponent<XpComponent>().Xp;
    }
}
