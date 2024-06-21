using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    public List<GameObject> MapList;
    string currentMap;


    Dictionary<int, int> loadedMapDic = new Dictionary<int, int>();

    public void Init(Tables.Dungeon dungeon = null)
    {
        string mapPrefabName = string.Empty;

        if (dungeon == null)
        {
            Tables.Stage stageTb = Stage.Get(AccountManager.Instance.CurrentStageInfo.key);
            mapPrefabName = stageTb.StagePrefabs;
        }
        if(!string.IsNullOrEmpty(mapPrefabName))
        {
            SetMapByName(mapPrefabName);
        }
    }
    public void SetMapByName(string _mapName)
    {
        if (!string.IsNullOrEmpty(_mapName) && currentMap != _mapName)
        {
            currentMap = _mapName;
            SetActiveMapList(false);

            Map map = new Map();

            int mapIndex = PoolManager.Instance.mapPrefabs.Maps.FindIndex(x => x.name == _mapName);
            if(mapIndex >= 0)
                map = PoolManager.Instance.mapPrefabs.Maps[mapIndex];

            if (map.mapList.Count == 0 || map.mapList[0] == null || loadedMapDic.Count == 0)
            {
                if (loadedMapDic.Count >= 10)
                {
                    int loadCount = 0;
                    int findKey = -1;
                    foreach (var mapData in loadedMapDic)
                    {
                        if (loadCount == 0 || mapData.Value < loadCount)
                        {
                            findKey = mapData.Key;
                            loadCount = mapData.Value;
                        }
                    }
                    DestroyMap(findKey);
                }
            }
            else
                loadedMapDic[mapIndex]++;

            MapList = map.mapList;


            PoolManager.Instance.mapPrefabs.SetMap(mapIndex);
            MapList = PoolManager.Instance.mapPrefabs.Maps[mapIndex].mapList;
            Navigation.Instance.CreateMap(map);

            SetActiveMapList(true);
        }

    }
    public void SetActiveMapList(bool isActive)
    {
        foreach (var item in MapList)
            item.SetActive(isActive);
    }

    void DestroyMap(int key)
    {
        loadedMapDic.Remove(key);
        foreach (var mapData in PoolManager.Instance.mapPrefabs.Maps[key].mapList)
        {
            Destroy(mapData);
        }
        PoolManager.Instance.mapPrefabs.Maps[key].mapList.Clear();
        PoolManager.Instance.mapPrefabs.Maps[key].mapList = new List<GameObject>();
    }
}
