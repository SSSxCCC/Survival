using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Online
{
    public class MapChooser : MonoBehaviour
    {
        public MapBundle mapBundle;

        public Text mapNameText;

        public Camera mapCamera;

        private int index;
        private GameObject currentMapInstance;
        private Dictionary<string, GameObject> mapInstanceDict = new Dictionary<string, GameObject>();

        // 一开始将选中状态与静态变量保持一致
        private void Start()
        {
            if (OnlineGameSetting.map != null && mapBundle.nameMapDict.ContainsKey(OnlineGameSetting.map))
                index = Array.IndexOf(mapBundle.mapPrefabs, mapBundle.nameMapDict[OnlineGameSetting.map]);
            else
                index = 0;

            UpdateMapInfo();
        }

        // 按下下一张地图按钮时调用
        public void NextMap()
        {
            if (index < mapBundle.mapPrefabs.Length - 1)
            {
                index++;
                UpdateMapInfo();
            }
        }

        // 按下上一张地图按钮时调用
        public void PrevMap()
        {
            if (index > 0)
            {
                index--;
                UpdateMapInfo();
            }
        }

        private void UpdateMapInfo()
        {
            MapInfo mapInfo = mapBundle.mapPrefabs[index].GetComponent<MapInfo>();
            mapNameText.text = mapInfo.name;
            OnlineGameSetting.map = mapInfo.name;

            if (currentMapInstance != null)
                currentMapInstance.SetActive(false);

            if (mapInstanceDict.ContainsKey(mapInfo.name))
            {
                currentMapInstance = mapInstanceDict[mapInfo.name];
                currentMapInstance.SetActive(true);
            }
            else
            {
                currentMapInstance = Instantiate(mapBundle.mapPrefabs[index]);
                mapInstanceDict[mapInfo.name] = currentMapInstance;
            }

            MapBorder mapBorder = currentMapInstance.GetComponentInChildren<MapBorder>();
            mapCamera.orthographicSize = Mathf.Max(mapBorder.mapHalfWidth, mapBorder.mapHalfHeight);
        }
    }
}