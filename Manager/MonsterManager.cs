using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : Singleton<MonsterManager>
{


    public Transform monsterRoot;
    public List<MonsterController> monsterList = new List<MonsterController>();

    MonsterController bossMon;
    int genMonsterStep;

    void Start()
    {
    }
    void Update()
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
            monsterCon.aniCtrl.CurrentState = OBJ_ANIMATION_STATE.IDLE;


            CapsuleCollider capsule = targetMonsterOjb.GetComponent<CapsuleCollider>();
            capsule.center = Vector3.up;
            capsule.radius = 0.4f;
            capsule.height = 2;
            capsule.direction = 1;
            capsule.enabled = true;

            targetMonsterOjb.SetActive(false);

            if (!isBoss)
            {
                monsterList.Add(monsterCon);
            }
            else
            {
                bossMon = monsterCon;
            }

        }
        else
        {
            Debug.LogError(string.Format("{0} is null", _index));
        }
    }

    public void CheckStageStep()
    {
        Stage curStage = Stage.Get(AccountManager.instance.CurStageKey);
        if(curStage != null)
        {
            //
            if(GameManager.Instance.StageStep >= genMonsterStep)
            {
                //보스도전을 누르면 보스를 도전하는 형식으로
                StartCoroutine(BossSpawn());
            }

        }
    }

    IEnumerator BossSpawn()
    {
        yield return null;
    }
}
