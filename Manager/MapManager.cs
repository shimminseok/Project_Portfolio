using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    public List<GameObject> MapList;
    public Map currentMapData;
    string currentMap;

    void Start()
    {
        PoolManager.Instance.CreateMap(0, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init(Tables.Dungeon dungeon = null)
    {
        string mapPrefabName = string.Empty;

        if (dungeon == null)
        {
            Tables.Stage stageTb = Stage.Get(AccountManager.Instance.CurStageKey);
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

            Map map = PoolManager.Instance.MapPrefabs.Maps.Find(x => x.name == _mapName);
            currentMapData = map;
            MapList = map.MapList;
            SetActiveMapList(true);
        }

    }
    public void SetActiveMapList(bool isActive)
    {
        foreach (var item in MapList)
            item.SetActive(isActive);
    }
}
