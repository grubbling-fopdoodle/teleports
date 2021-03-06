﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Teleports.Utils;

[Serializable]
public class InventoryItemSpawner
{
    private static GameObject spawnedItemsContainer;

    [SerializeField] List<GameObject> itemPrefabs;
    [SerializeField] List<GameObject> spawnedItems;

    public InventoryItemSpawner(List<GameObject> itemPrefabs)
    {
        this.itemPrefabs = itemPrefabs;
        spawnedItems = new List<GameObject>();
    }

    public void Spawn()
    {
        WorldPositionGrid spawnPlace = SpecialSpawnPlaces.ItemSpawnPlace;
        Despawn();

        foreach(GameObject itemPrefab in itemPrefabs)
        {
            GameObject spawnedItem = GameObject.Instantiate(itemPrefab) as GameObject;
            spawnedItem.transform.position = spawnPlace.CurrentPosition;
            spawnedItem.transform.parent = SpawnedItemsContainer.transform;
            spawnedItems.Add(spawnedItem);
            spawnPlace.GoToNextPosition();
        }
    }

    public void Despawn()
    {
        foreach(var item in spawnedItems)
        {
            if (item != null)
            {
                item.SetActive(false);
                GameObject.Destroy(item);
            }
        }
    }

    public MeshFilter GetItemMeshFilter(int internalItemId)
    {
        return spawnedItems[internalItemId].GetComponent<MeshFilter>();
    }

    public IList<GameObject> SpawnedItems => spawnedItems.AsReadOnly();

    private GameObject SpawnedItemsContainer
    {
        get
        {
            if (spawnedItemsContainer == null)
            {
                spawnedItemsContainer = new GameObject("Inventory - SpawnedItems");
            }
            return spawnedItemsContainer;
        }
    }
}
