using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.Events;

public class PoolManager : Singleton<PoolManager>
{

    [HideInInspector] bool IsCreatedMaps = false;
    public Transform poolRoot;
    public MapPrefabs mapPrefabs;

    Dictionary<string, List<GameObject>> poolObjects = new Dictionary<string, List<GameObject>>();
    Dictionary<string, GameObject> poolObject = new Dictionary<string, GameObject>();

    void Start()
    {
        Stage stageTb = Stage.Get(AccountManager.Instance.CurStageKey);
        Spawn spawnTb = Spawn.Get(stageTb.SpawnGroup);
        for (int i = 0; i < spawnTb.MonsterIndex.Length; i++)
        {
            CreateMonsterPool(Monster.Get(spawnTb.MonsterIndex[i]), null);
        }
    }
    public void CreateMonsterPool(Tables.Monster _monster, UnityAction _action)
    {
        try
        {
            CreateObj(_monster.Prefabs, POOL_TYPE.MONSTER, 10);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Create Monster Fail Name : {0}", _monster.Monster_Name);
        }
        _action?.Invoke();
    }
    public void CreateObj(string _name, POOL_TYPE _poolType,int _count)
    {
        GameObject parent = null;
        if (poolObject.ContainsKey(_name))
        {
            if (poolObjects[_name].Count > 0)
                return;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Equals(_name + "Parent"))
                    parent = transform.GetChild(i).transform.gameObject;
            }
        }
        else
        {
            string path = string.Empty;

            switch (_poolType)
            {
                case POOL_TYPE.MONSTER: path = string.Format("Prefabs/Monster/{0}", _name); break;
                case POOL_TYPE.MAP: path = _name; break;
                default: break;
            }

            poolObject[_name] = Resources.Load<GameObject>(path);
            poolObjects[_name] = new List<GameObject>();
            parent = new GameObject();
            parent.transform.parent = poolRoot;
            parent.name = string.Format("{0}Parent", _name);
        }

        if (poolObject[_name] == null)
        {
            Debug.LogError(string.Format("{0} in null in CreateObj", _name));
            return;
        }

        for (int i = 0; i < _count; i++)
        {
            GameObject pool = Instantiate(poolObject[_name], parent.transform);
            pool.name = _name;
            switch(_poolType)
            {
                case POOL_TYPE.MONSTER:
                    {
                        Tables.Monster monsterTb = null;
                        foreach(var monster in Tables.Monster.data.Values)
                        {
                            if(monster.Prefabs == _name)
                            {
                                monsterTb = monster;
                                pool.AddComponent<MonsterController>().monsterTb = monsterTb;
                                pool.transform.localScale *= monster.Scale;

                                break;
                            }
                        }
                    }
                    break;
                case POOL_TYPE.MAP:
                    break;
            }
            pool.transform.localPosition = Vector3.zero;
            pool.transform.localRotation = Quaternion.identity;
            pool.SetActive(false);
            poolObjects[_name].Add(pool);
        }
    }


    public void PushObj(string _name, POOL_TYPE _type, GameObject _gameObject)
    {
        CreateObj(_name, _type, 1);
        GameObject parent = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == _name + "Parent")
                parent = transform.GetChild(i).transform.gameObject;
        }

        _gameObject.transform.parent = parent.transform;
        _gameObject.transform.localPosition = Vector3.zero;
        _gameObject.transform.localRotation = Quaternion.identity;
        _gameObject.SetActive(false);
        poolObjects[_name].Add(_gameObject);
    }
    public GameObject GetObj(string _name, POOL_TYPE _type, int _count = 1)
    {
        CreateObj(_name, _type, _count);

        GameObject go = poolObjects[_name][0];
        poolObjects[_name].RemoveAt(0);

        go.transform.position = Vector3.zero;
        go.SetActive(true);
        return go;
    }
}
