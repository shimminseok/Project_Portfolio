using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "MonsterPrefabs", menuName = "ScriptableObject/MonsterPrefabs")]
public class MonsterPrefabs : ScriptableObject
{
    public GameObject monsterControlPrefab;

    public List<GameObject> MonsterPrefabList = new List<GameObject>();
}
