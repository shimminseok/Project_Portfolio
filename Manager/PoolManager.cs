using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{

    [HideInInspector] bool IsCreatedMaps = false;
    public Transform poolRoot;
    public MapPrefabs mapPrefabs;

    Dictionary<string, Queue<GameObject>> poolObjects = new Dictionary<string, Queue<GameObject>>();
    Dictionary<string, GameObject> poolObject = new Dictionary<string, GameObject>();


    string urlEffect = "Effect/CommonEffect/";
    string urlEffectAdd = "Effect/CommonEffect/{0}";
    string urlEffectMonsterAtk = "Model/Monster/{0}/Effect/{0}_Atk_FX";
    string urlEffectMonsterAtkHit = "Model/Monster/{0}/Effect/{0}_Atk_Hit_FX";
    string urlEffectMonsterMissile = "Model/Monster/{0}/Effect/{0}_Atk_Missile_FX";
    string urlEffectMonsterSkill01 = "Model/Monster/{0}/Effect/{0}_Skill01_FX";
    string urlEffectMonsterSkill01Hit = "Model/Monster/{0}/Effect/{0}_Skill01_Hit_FX";
    string urlEffectMonsterSkill01Missile = "Model/Monster/{0}/Effect/{0}_Skill01_Missile_FX";
    string urlEffectMonsterSkill01Target = "Model/Monster/{0}/Effect/{0}_Skill01_Target_FX";


    public void CreateObj(string _name, POOL_TYPE _poolType, int _count)
    {
        GameObject parent = null;
        if (poolObject.ContainsKey(_name))
        {
            if (poolObjects[_name].Count > 0)
                return;

            for (int i = 0; i < poolRoot.transform.childCount; i++)
            {
                if (poolRoot.transform.GetChild(i).name.Equals(_name + "Parent"))
                {
                    parent = poolRoot.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        else
        {
            string path = string.Empty;

            switch (_poolType)
            {
                case POOL_TYPE.MONSTER: path = string.Format("Model/Monster/{0}/{0}", _name); break;
                case POOL_TYPE.MAP: path = _name; break;
                case POOL_TYPE.TAG: path = string.Format("Prefabs/Tag/{0}", _name); break;
                case POOL_TYPE.EFFECT: path = string.Format("Effect/{0}", _name); break;
                default: break;
            }
            poolObject[_name] = Resources.Load<GameObject>(path);
            poolObjects[_name] = new Queue<GameObject>();
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
            switch (_poolType)
            {
                case POOL_TYPE.MONSTER:
                    {
                        Tables.Monster monsterTb = null;
                        foreach (var monster in Tables.Monster.data.Values)
                        {
                            if (monster.Prefabs == _name)
                            {
                                monsterTb = monster;
                                if (monsterTb != null)
                                {
                                    MonsterController monctrl = pool.GetComponent<MonsterController>();
                                    if (monctrl == null)
                                        monctrl = pool.AddComponent<MonsterController>();

                                    monctrl.monsterTb = monsterTb;
                                    pool.transform.localScale *= monster.Scale;

                                    monctrl.aniCtrl.EffectList = new GameObject[(int)EFFECT_TYPE.MAX].ToList();
                                    // 죽음
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.DEAD] = Resources.Load<GameObject>(string.Format(urlEffectAdd, monster.Death_Effect));

                                    // 평타 
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.ATTACK] = Resources.Load<GameObject>(string.Format(urlEffectMonsterAtk, _name));

                                    // 평타 타격
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.HIT] = Resources.Load<GameObject>(string.Format(urlEffectMonsterAtkHit, _name));
                                    if (monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.HIT] == null)
                                        monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.HIT] = Resources.Load<GameObject>(urlEffect + "atk_hit_fx_1");

                                    // 평타 발사체
                                    if (monctrl.TryGetComponent<IAttackable>(out IAttackable iatk))
                                    {
                                        if (iatk.IsRangeAttacker)
                                        {
                                            monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.PROJECTILE] = Resources.Load<GameObject>(string.Format(urlEffectMonsterMissile, _name));
                                            if (monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.PROJECTILE] == null)
                                                monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.PROJECTILE] = Resources.Load<GameObject>(urlEffect + "Dr_Mon_1140_atk_trail_FX");
                                        }
                                    }

                                    // 스킬1
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.SKILL1] = Resources.Load<GameObject>(string.Format(urlEffectMonsterSkill01, _name));

                                    // 스킬1 타격 
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.SKILL1_HIT] = Resources.Load<GameObject>(string.Format(urlEffectMonsterSkill01Hit, _name));

                                    // 스킬1 발사체
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.SKILL1_MISSILE_FX] = Resources.Load<GameObject>(string.Format(urlEffectMonsterSkill01Missile, _name));

                                    // 스킬1 타겟 // 힐, 폭팔마법 등 
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.SKILL1_TARGET_FX] = Resources.Load<GameObject>(string.Format(urlEffectMonsterSkill01Target, _name));

                                    // 보스 타격 이펙트 추가
                                    monctrl.aniCtrl.EffectList[(int)EFFECT_TYPE.HIT_BOSS] = Resources.Load<GameObject>(urlEffect + "Hit_Slash_Boss_FX");
                                }
                                else
                                    Debug.LogWarningFormat("Monster Table is Null");

                                break;
                            }
                        }
                    }
                    break;
                case POOL_TYPE.MAP:
                    break;
                case POOL_TYPE.TAG:
                    break;
                case POOL_TYPE.EFFECT:
                    break;
            }
            pool.transform.localPosition = Vector3.zero;
            pool.transform.localRotation = Quaternion.identity;
            pool.SetActive(false);
            poolObjects[_name].Enqueue(pool);
        }
    }


    public void PushObj(string _name, POOL_TYPE _type, GameObject _gameObject)
    {
        CreateObj(_name, _type, 1);
        GameObject parent = null;

        for (int i = 0; i < poolRoot.childCount; i++)
        {
            if (poolRoot.GetChild(i).name == _name + "Parent")
            {
                parent = poolRoot.GetChild(i).transform.gameObject;
                break;
            }
        }
        switch (_type)
        {
            case POOL_TYPE.TAG:
                _gameObject.transform.SetParent(parent.transform);
                break;
            default:
                _gameObject.transform.parent = parent.transform;
                break;
        }
        _gameObject.transform.localPosition = Vector3.zero;
        _gameObject.transform.localRotation = Quaternion.identity;
        _gameObject.SetActive(false);
        poolObjects[_name].Enqueue(_gameObject);
    }
    public GameObject GetObj(string _name, POOL_TYPE _type, int _count = 1)
    {
        CreateObj(_name, _type, _count);

        GameObject go = poolObjects[_name].Dequeue();
        if (go != null)
        {
            go.transform.position = Vector3.zero;
            go.SetActive(true);
        }
        return go;
    }
}
