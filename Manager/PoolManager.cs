using UnityEngine;
using UnityEngine.Events;

public class PoolManager : Singleton<PoolManager>
{

    [HideInInspector] public bool IsCreatedMaps = false;
    public Transform mapPoolRoot;
    public MapPrefabs mapPrefabs;


    public void CreateMap(int index, UnityAction action)
    {
        mapPrefabs.SetMap(index);
        action?.Invoke();
    }

    public void CreateMonster(int key)
    {
    }
}
