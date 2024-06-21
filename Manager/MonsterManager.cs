using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    public Transform monsterRoot;
    public List<MonsterController> monsterList = new List<MonsterController>();

    private Tables.Stage currentStageTb;
    private MonsterController bossMon;

    private int genMonsterStep;
    private int stageStep = 1;


    public MonsterController BossMon => bossMon;
    public int GenMonsterStep => genMonsterStep;
    public int StageStep => stageStep;
    public Tables.Stage CurrentStageTb
    {
        get => currentStageTb;
        set
        {
            currentStageTb = value;
            genMonsterStep = currentStageTb.SpawnCount;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        stageStep = 0;
        monsterList.Clear();
        CurrentStageTb = Stage.Get(AccountManager.Instance.CurrentStageInfo.key);
        for (int i = monsterRoot.childCount - 1; i >= 0; i--)
        {
            GameObject pushObj = monsterRoot.GetChild(i).gameObject;
            if (pushObj.TryGetComponent(out MonsterController monCtrl))
            {
                monCtrl.IsDead = true;
                monCtrl.PushObj();
            }
        }
    }

    public void CreateMonsterPool(int _index, Vector3 _genPos, bool isBoss = false)
    {
        Monster MonsterTb = Monster.Get(_index);
        if (MonsterTb == null)
        {
            Debug.LogError($"{_index} is not Exist in Monster Table");
            return;
        }

        GameObject targetMonsterObj = PoolManager.Instance.GetObj(MonsterTb.Prefabs, POOL_TYPE.MONSTER);
        if (targetMonsterObj == null)
        {
            Debug.LogError($"{_index} is null");
            return;
        }

        targetMonsterObj.layer = LayerMask.NameToLayer("Monster");
        targetMonsterObj.transform.SetParent(monsterRoot);
        targetMonsterObj.transform.localScale = Vector3.one;

        MonsterController monsterCon = targetMonsterObj.GetComponent<MonsterController>();
        SetupCapsuleCollider(targetMonsterObj);

        if (isBoss)
        {
            bossMon = monsterCon;
            monsterCon.SetMonster(_genPos, MONSTER_TYPE.BOSS, currentStageTb.BossLv);
        }
        else
        {
            monsterCon.SetMonster(_genPos, MONSTER_TYPE.COMMON, currentStageTb.MonsterLv);
        }

        monsterList.Add(monsterCon);
    }

    void SetupCapsuleCollider(GameObject targetMonsterObj)
    {
        if (!targetMonsterObj.TryGetComponent<CapsuleCollider>(out var capsule))
        {
            capsule = targetMonsterObj.AddComponent<CapsuleCollider>();
        }

        capsule.center = Vector3.up;
        capsule.radius = 0.4f;
        capsule.height = 2;
        capsule.direction = 1;
        capsule.enabled = true;
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

        if (currentStageTb != null)
        {
            if (stageStep >= genMonsterStep && !AccountManager.Instance.CurrentStageInfo.isChallengeableBoss)
            {
                AccountManager.Instance.CurrentStageInfo.isChallengeableBoss = true;
                ChallengeBoss();
            }
            else
            {
                MonsterSpawn();
            }
            UIManager.Instance.SetWaveProsessUI();
        }
    }

    void MonsterSpawn()
    {
        List<Vector3> spawnPoints = Navigation.Instance.monsterSpawnPoints;
        int index = 0;

        for (int i = 0; i < 5; i++)
        {
            Tables.Spawn spawnTb = Tables.Spawn.Get(currentStageTb.SpawnGroup);
            if (spawnTb != null)
            {
                int spawnMonIndex = Random.Range(0, spawnTb.MonsterIndex.Length);
                CreateMonsterPool(spawnTb.MonsterIndex[spawnMonIndex], spawnPoints[index++]);

                if (index >= spawnPoints.Count)
                {
                    index = 0;
                }
            }
        }
    }

    public void ChallengeBoss()
    {
        if (AccountManager.Instance.CurrentStageInfo.isChallengeableBoss)
        {
            StartCoroutine(BossSpawn());
        }
    }

    IEnumerator BossSpawn()
    {
        Init();
        stageStep = 1;
        CreateMonsterPool(currentStageTb.BossIndex, Navigation.Instance.boss.worldPos, true);
        GameManager.Instance.ChangeGameState(GAME_STATE.BOSS);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.ChangeGameState(GAME_STATE.PLAYING);
    }
}
