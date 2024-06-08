using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    public Transform monsterRoot;
    public List<MonsterController> monsterList = new List<MonsterController>();
    public Tables.Stage currentStageTb;

    MonsterController bossMon;


    int genMonsterStep;
    int stageStep = 1;

    public bool isChallengeableBoss;

    public MonsterController BossMon => bossMon;

    public int GenMonsterStep => genMonsterStep;
    public int StageStep => stageStep;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
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
                monsterCon.SetMonster(_genPos, MONSTER_TYPE.COMMON,currentStageTb.MonsterLv);
                monsterList.Add(monsterCon);
            }
            else
            {
                bossMon = monsterCon;
                monsterCon.SetMonster(_genPos, MONSTER_TYPE.BOSS,currentStageTb.BossLv);
                monsterList.Add(monsterCon);
            }
        }
        else
        {
            Debug.LogError(string.Format("{0} is null", _index));
        }
    }

    public void RemoveMonsterList(MonsterController _target)
    {
        monsterList.Remove(_target);

        if (monsterList.Count == 0 && GameManager.Instance.GameState == GAME_STATE.PLAYING)
        {
            stageStep++;
            MonsterRegen();
        }
    }

    public void MonsterRegen()
    {
        if (monsterList.Count > 0)
            return;

        currentStageTb = Stage.Get(AccountManager.Instance.CurStageKey);
        if (currentStageTb != null)
        {
            genMonsterStep = currentStageTb.SpawnCount;

            if (stageStep >= genMonsterStep && !isChallengeableBoss)
            {
                isChallengeableBoss = true;
                ChallengeBoss();
            }
            else
            {
                List<Vector3> spawnPoint = Navigation.Instance.monsterSpawnPoints.Keys.ToList();
                for (int i = 0; i < 5; i++)
                {
                    int randomPointIndex = Random.Range(0, spawnPoint.Count);

                    Tables.Spawn spwanTb = Tables.Spawn.Get(Navigation.Instance.monsterSpawnPoints[spawnPoint[randomPointIndex]]);
                    if (spwanTb != null)
                    {
                        int spwanMonIndex = Random.Range(0, spwanTb.MonsterIndex.Length);
                        CreateMonsterPool(spwanTb.MonsterIndex[spwanMonIndex], spawnPoint[randomPointIndex]);
                    }
                }
            }
            UIManager.Instance.SetWaveProsessUI();
        }
    }
    public void ChallengeBoss()
    {
        if (isChallengeableBoss)
        {
            StartCoroutine(BossSpawn());
        }
    }
    IEnumerator BossSpawn()
    {
        stageStep = 1;
        Vector3 genPos = new Vector3(Navigation.Instance.start.Position.x - 10, 0, Navigation.Instance.start.Position.z + 10);
        CreateMonsterPool(currentStageTb.BossIndex, genPos, true);
        GameManager.Instance.ChangeGameState(GAME_STATE.BOSS);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.ChangeGameState(GAME_STATE.PLAYING);

    }
}
