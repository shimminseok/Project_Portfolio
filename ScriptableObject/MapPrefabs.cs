using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "MapPrefabs", menuName = "ScriptableObject/MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public List<Map> Maps = new List<Map>();

    public void SetMap(int index)
    {
        PoolManager.Instance.mapPoolRoot.localPosition = new Vector3(-(Maps[index].GroundSize.x / 2), 0, -(Maps[index].GroundSize.y / 2));
        Maps[index].MapNode = new Node[(int)(Maps[index].GroundSize.x), (int)(Maps[index].GroundSize.y)];
        for (int i = 0; i < Maps[index].GroundSize.x; i++)
        {
            for (int j = 0; j < Maps[index].GroundSize.y; j++)
            {
                //맵을 생성해 줘야함
                //임시로 맵에 장애물이 없는 형태로 생성
                GameObject go = Instantiate(Random.value <= 1f ? Maps[index].MapList[^1] : Maps[index].MapList[Random.Range(0, Maps[index].MapList.Count - 1)],PoolManager.Instance.mapPoolRoot);
                go.transform.localPosition = new Vector3(i * 1f, -0.5f, j * 1f);
                Maps[index].MapNode[i, j] = new Node(i, j, go.layer == LayerMask.NameToLayer("IsMoveable"));
            }
        }
    }
}
