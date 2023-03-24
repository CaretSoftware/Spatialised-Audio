using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    [SerializeField] private float[] ceilingHeight;
    [SerializeField] private Sprite[] floorSprites;
    [SerializeField] private Image map;
    [SerializeField] private RectTransform playerIcon;
    [SerializeField] private RectTransform mapsRect;
    [SerializeField] private Transform player;
    [SerializeField] private Vector2 mapsMin;
    [SerializeField] private Vector2 mapsMax;
    [SerializeField] private Vector2 posMin;
    [SerializeField] private Vector2 posMax;

    private void Update() {
        ActivateCurrentFloor();
        RotatePlayerIcon();
        MoveMap();
    }

    private void MoveMap() {
        Vector3 playerPos = player.position;
        Vector2 pos = new Vector2(playerPos.x, playerPos.z);

        float invX = Mathf.InverseLerp(posMin.x, posMax.x, pos.x);
        float invY = Mathf.InverseLerp(posMin.y, posMax.y, pos.y);

        float x = Mathf.Lerp(mapsMin.x, mapsMax.x, invX);
        float y = Mathf.Lerp(mapsMin.y, mapsMax.y, invY);

        mapsRect.anchoredPosition = new Vector2(x, y);
    }

    private void ActivateCurrentFloor() {
        map.sprite = floorSprites[FloorNumber(player)];

        int FloorNumber(Transform trans) {
            for (int i = 0; i < ceilingHeight.Length; i++) {
                if (trans.position.y < ceilingHeight[i])
                    return i;
            }

            return -1;
        }
    }

    private void RotatePlayerIcon() {
        float rotation = -player.rotation.eulerAngles.y;
        playerIcon.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
    }
}
