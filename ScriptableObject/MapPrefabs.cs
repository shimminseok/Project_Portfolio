using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "MapPrefabs", menuName = "ScriptableObject/MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public List<Map> Maps = new List<Map>();
    GameObject mapRoot;

    public Map SetMap(int index)
    {
        //if(mapRoot == null)
        //{
        //    mapRoot = new GameObject();
        //    mapRoot.name = "MapObjRoot";
        //    mapRoot.transform.parent = PoolManager.Instance.poolRoot;
        //    mapRoot.transform.localPosition = new Vector3(-(Maps[index].GroundSize.x / 2), 0, -(Maps[index].GroundSize.y / 2));
        //}
        Maps[index].MapNode = new Node[(int)(Maps[index].GroundSize.x), (int)(Maps[index].GroundSize.y)];
        return Maps[index];
    }
}
