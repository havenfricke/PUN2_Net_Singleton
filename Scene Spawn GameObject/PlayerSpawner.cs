using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public sealed class PlayerSpawner : MonoBehaviour
{
    [Header("Name of the player prefab (In resources folder)")]
    [SerializeField] private string playerPrefabName = "Player";

    [Header("Spawn Transforms")]
    [SerializeField] private Transform[] spawnPoints;

    private Dictionary<Transform, bool> spawnAvailability = new Dictionary<Transform, bool>();

    private bool hasSpawned = false;

    private void Awake()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
            {
                spawnAvailability.Add(spawnPoints[i], true);
            }
        }
    }

    private void OnEnable()
    {
        Net.Instance.JoinedRoom += HandleJoinedRoom;
        Net.Instance.LeftRoom += HandleLeftRoom;
        Net.Instance.Disconnected += HandleDisconnected;
    }

    private void OnDisable()
    {
        if (Net.Instance != null)
        {
            Net.Instance.JoinedRoom -= HandleJoinedRoom;
            Net.Instance.LeftRoom -= HandleLeftRoom;
            Net.Instance.Disconnected -= HandleDisconnected;
        }
    }

    private void Start()
    {
        // If the scene loads and the player is already in the room, trigger spawn logic immediately
        if (Net.Instance.InRoom)
        {
            HandleJoinedRoom();
        }
    }

    private async void HandleJoinedRoom()
    {
        if (hasSpawned) return;

        try
        {
            // staggering gives the network time to sync colliders across clients
            int randomStagger = Random.Range(100, 1000);
            await Task.Delay(randomStagger, destroyCancellationToken);

            // Keep looping until we successfully spawn
            while (!hasSpawned)
            {
                if (TryChooseAvailableSpawn(out Transform chosenSpawn, out Vector3 position, out Quaternion rotation))
                {
                    SetSpawnAvailability(chosenSpawn, false);

                    object[] data = new object[] { Net.Instance.LocalPlayer.NickName };

                    Net.Instance.Instantiate(
                        prefabName: playerPrefabName,
                        position: position,
                        rotation: rotation,
                        group: 0,
                        data: data
                    );

                    hasSpawned = true;
                }
                else
                {
                    // wait if all dictionary spots false
                    await Task.Delay(250);
                }
            }
        } 
        catch (TaskCanceledException)
        {
            return;
        }
        
    }

    private void HandleLeftRoom()
    {
        hasSpawned = false;
    }

    private void HandleDisconnected()
    {
        hasSpawned = false;
    }

    // actively filter for available spots
    private bool TryChooseAvailableSpawn(out Transform chosenSpawn, out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        chosenSpawn = null;

        List<Transform> availableSpawns = new List<Transform>();

        // gather spawns that are marked true
        foreach (var kvp in spawnAvailability)
        {
            if (kvp.Value == true)
            {
                availableSpawns.Add(kvp.Key);
            }
        }

        if (availableSpawns.Count == 0) return false;

        // random selection from the safe list
        int index = Random.Range(0, availableSpawns.Count);
        chosenSpawn = availableSpawns[index];
        position = chosenSpawn.position;
        rotation = chosenSpawn.rotation;

        return true;
    }

    public void SetSpawnAvailability(Transform spawnPoint, bool isAvailable)
    {
        if (spawnAvailability.ContainsKey(spawnPoint))
        {
            spawnAvailability[spawnPoint] = isAvailable;
        }
    }
}