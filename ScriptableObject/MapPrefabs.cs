using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "MapPrefabs", menuName = "ScriptableObject/MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public List<Map> Maps = new List<Map>();

    public void SetMap(int index)
    {
        int width =20;
        int height = 20;
        Maps[index].GroundSize = new Vector2(width, height);
        Maps[index].MapNode = new Node[(int)(Maps[index].GroundSize.x), (int)(Maps[index].GroundSize.y)];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Maps[index].start.Position = new Vector3(i * 5,0,j * 5);
            }
        }

        //SetManualMap(index);
    }

    void SetManualMap(int index)
    {
        string STR_PATH_MAP_PIECE_A = "Prefabs/Map/DW_BG_014A";
        string STR_PATH_MAP_PIECE_B = "Prefabs/Map/DW_BG_014B";
        string STR_MAP_PIECE = "DW_BG_014";

        // 갱도 던전 맵 수동으로 만들기
        if (Maps[index].name == STR_MAP_PIECE)
        {
            GameObject Parent = CreateObj(null, Quaternion.Euler(0, 45, 0), new Vector3(0, -0.01f, 0), PoolManager.Instance.transform, true);

            Maps[index].MapList.Clear();

            for (int i = 1; i <= 4; i++)
                Maps[index].MapList.Add(CreateObj((i % 2 == 0) ? STR_PATH_MAP_PIECE_B : STR_PATH_MAP_PIECE_A, Quaternion.Euler(0, -90, 0), Vector3.zero, Parent.transform, false));
        }
    }

    GameObject CreateObj(string path, Quaternion quaternion, Vector3 position, Transform parent, bool isActive)
    {
        GameObject go = (path == null) ? new GameObject() : Instantiate(Resources.Load<GameObject>(path));
        go.transform.parent = parent;
        go.transform.localRotation = quaternion;
        go.transform.localPosition = (path == null) ? go.transform.localPosition - position : position;
        go.SetActive(isActive);

        return go;
    }
}
