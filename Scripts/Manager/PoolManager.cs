using UnityEngine;
using UnityEngine.Events;

public class PoolManager : Singleton<PoolManager>
{
    [HideInInspector] public bool IsCreatedMaps = false;
    public MapPrefabs MapPrefabs;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CreateMap(int index, UnityAction action)
    {
        MapPrefabs.SetMap(index);
        action();
    }
}
