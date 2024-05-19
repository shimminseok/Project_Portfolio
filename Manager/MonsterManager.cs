using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    public Transform monsterRoot;
    public List<MonsterController> monsterList = new List<MonsterController>();
    public Tables.Stage currentStageTb;

    MonsterController bossMon;
    int genMonsterStep;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
    }
    void Update()
    {
        if(monsterList.Count <= 2)
        {
            CheckStageStep();
        }
    }

    public void CreateMonsterPool(int _index, Vector3 _genPos, bool isBoss = false)
    {
        MonsterController monsterCon = null;
        Monster MonsterTb = Monster.Get(_index);
        if (MonsterTb == null)
        {
            Debug.LogError(string.Format("{0} is not Exist in Monster Table", _index));
            return;
        }

        GameObject targetMonsterOjb = null;

        targetMonsterOjb = PoolManager.Instance.GetObj(MonsterTb.Prefabs, POOL_TYPE.MONSTER);

        if (targetMonsterOjb != null)
        {
            targetMonsterOjb.transform.parent = monsterRoot;

            targetMonsterOjb.transform.localScale = Vector3.one;
            monsterCon = targetMonsterOjb.GetComponent<MonsterController>();


            CapsuleCollider capsule = targetMonsterOjb.GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                capsule = targetMonsterOjb.AddComponent<CapsuleCollider>();
            }
            capsule.center = Vector3.up;
            capsule.radius = 0.4f;
            capsule.height = 2;
            capsule.direction = 1;
            capsule.enabled = true;

            targetMonsterOjb.SetActive(true);

            if (!isBoss)
            {
                monsterCon.SetMonster(_genPos, MONSTER_TYPE.COMMON);
                monsterList.Add(monsterCon);
            }
            else
            {
                bossMon = monsterCon;
                monsterCon.SetMonster(_genPos, MONSTER_TYPE.BOSS);
            }
        }
        else
        {
            Debug.LogError(string.Format("{0} is null", _index));
        }
    }

    public void CheckStageStep()
    {
        currentStageTb = Stage.Get(AccountManager.Instance.CurStageKey);
        if(currentStageTb != null)
        {
            genMonsterStep = currentStageTb.SpawnCount;

            if (GameManager.Instance.stageStep >= genMonsterStep)
            {
                //보스도전을 누르면 보스를 도전하는 형식으로
                StartCoroutine(BossSpawn());
            }
            else
            {
                GameManager.Instance.stageStep = 0;
            }
            if(GameManager.Instance.stageStep == 0)
            {
                List<Vector3> spawnPoint = Navigation.Instance.monsterSpawnPoints.ToList();
                for (int i = 0; i < genMonsterStep; i++)
                {
                    int randomPointIndex = Random.Range(0, spawnPoint.Count);

                    Tables.Spawn spwanTb = Tables.Spawn.Get(currentStageTb.SpawnGroup);
                    if(spwanTb != null)
                    {
                        int spwanMonIndex = Random.Range(0, spwanTb.MonsterIndex.Length);
                        CreateMonsterPool(spwanTb.MonsterIndex[spwanMonIndex], spawnPoint[randomPointIndex]);
                    }
                }

            }

        }
    }

    IEnumerator BossSpawn()
    {
        yield return null;
    }
}
