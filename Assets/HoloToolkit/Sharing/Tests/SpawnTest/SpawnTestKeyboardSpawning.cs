//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using HoloToolkit.Sharing.SyncModel;
using HoloToolkit.Sharing.Spawning;
using UnityEngine;

/// <summary>
/// Class that handles spawning objects on keyboard presses, for the spawning test scene of
/// the HoloToolkit's Sharing component.
/// </summary>
public class SpawnTestKeyboardSpawning : MonoBehaviour
{
    public GameObject SpawnParent;
    public GameObject Prefab;

    public PrefabSpawnManager SpawnManager;

    private void Update()
    {
        // Spawn a basic spawned object when I is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            Vector3 position = Random.onUnitSphere * 2;
            Quaternion rotation = Random.rotation;

            SyncSpawnedObject spawnedObject = new SyncSpawnedObject();

            SpawnManager.Spawn(spawnedObject, position, rotation, SpawnParent, "SpawnedObject", false);
        }

        // Spawn a test sphere when O is pressed
        if (Input.GetKeyDown(KeyCode.O))
        {
            Vector3 position = Random.onUnitSphere * 2;
            Quaternion rotation = Random.rotation;

            SyncSpawnTestSphere spawnedObject = new SyncSpawnTestSphere();
            spawnedObject.TestFloat.Value = Random.Range(0f, 100f);

            SpawnManager.Spawn(spawnedObject, position, rotation, SpawnParent, "SpawnTestSphere", false);
        }
    }
}
