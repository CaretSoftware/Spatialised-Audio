﻿using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour {
    public delegate void RespawnGhost();
    public static RespawnGhost respawnGhost;

    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] public Transform[] spawnVolumes;
    private Dictionary<Transform, float> _spawnVolumes = new Dictionary<Transform, float>();
    private float _spawnVolumeTotal;
    private PositionWithinCube _positionWithinCube;
    public bool debug;

    public static Transform activeGhost;

    private Graph _graph;
    
    private void Awake() {
        _graph = FindObjectOfType<Graph>();
        respawnGhost += SpawnNewGhost;
        Initialize();
    }

    private void OnDestroy() {
        respawnGhost -= SpawnNewGhost;
    }

    private void SpawnNewGhost() {
        if (activeGhost != null)
            Destroy(activeGhost.gameObject);
        
        Vector3 spawnPosition = _graph.SpawnPosition();
        //RandomSpawnPosition(out Vector3 position);

        InstantiateGhost(spawnPosition);
        GhostAudio.newPosition?.Invoke(spawnPosition);
    }

    private void InstantiateGhost(Vector3 position) {
        Quaternion lookRotation = 
            Quaternion.LookRotation(
                -Vector3.ProjectOnPlane(position, Vector3.up).normalized, 
                Vector3.up);
        activeGhost = Instantiate(ghostPrefab, position, lookRotation).transform;
        OffScreenArrowIndicator.setTarget?.Invoke(activeGhost);
    }

    private void Start() {
        _positionWithinCube = GetComponent<PositionWithinCube>();
    }

    public Transform RandomSpawnPosition(out Vector3 spawnPosition) {
        Transform cube = GetWeightedRandomSpawnVolume();
        
        
        Vector3 spawnPos = _positionWithinCube.PositionWithin(cube);
        debugSpawnPosition = spawnPos; // debug
        spawnPosition = spawnPos;

        return cube;
    }
    
    private Transform GetWeightedRandomSpawnVolume() {
        float rand = Random.Range(0, _spawnVolumeTotal);
        Transform spawnVolume = null;
        
        for (int i = 0; i < spawnVolumes.Length; i++) {
            spawnVolume = spawnVolumes[i];
            float volume = _spawnVolumes[spawnVolume];
            if (rand <= _spawnVolumes[spawnVolume]) {
                debugSpawnBox.pos = spawnVolume.position;
                debugSpawnBox.scl = spawnVolume.localScale;

                return spawnVolume;
            }

            rand -= volume;
        }

        return null;
    }
    
    private void TestDebug() {
        int[] roomPicks = new int[spawnVolumes.Length];
        for (int i = 0; i < 1000 * spawnVolumes.Length; i++) {
            Transform vol = GetWeightedRandomSpawnVolume();
            for (int j = 0; j < spawnVolumes.Length; j++) {
                if (vol == spawnVolumes[j]) {
                    roomPicks[j] += 1;
                    break;
                }
            }
        }
        
        StringBuilder str = new StringBuilder("val: ");
        for (int i = 0; i < roomPicks.Length; i++) {
            str.Append($"[{roomPicks[i] / _spawnVolumes[spawnVolumes[i]]}] ");
        }
        
        Debug.Log(str.ToString());
    }

    private (Vector3 pos, Vector3 scl) debugSpawnBox;
    private Vector3 debugSpawnPosition;
    private Color spawnColor = new Color(1f, 1f, 0f, 1f);
    private Color cubeColor = new Color(0f, 0f, 1f, .5f);

    private void OnDrawGizmos() {
        Gizmos.color = spawnColor;
        Gizmos.DrawSphere(debugSpawnPosition, .5f);
        
        Gizmos.color = cubeColor;
        Gizmos.DrawCube(debugSpawnBox.pos, debugSpawnBox.scl);
    }

    private void Initialize() {
        float totalVolume = 0f;

        for (int i = 0; i < spawnVolumes.Length; i++) {
            
            float volume = Volume(spawnVolumes[i].transform);
            _spawnVolumes[spawnVolumes[i]] = volume;
            
            totalVolume += volume;
        }

        _spawnVolumeTotal = totalVolume;
        
        float Volume(Transform vol) {
            Vector3 scale = vol.localScale;
            return scale.x * scale.y * scale.z;
        }
    }
}
