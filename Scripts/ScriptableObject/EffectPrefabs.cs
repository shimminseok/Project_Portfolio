using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectPrefabs", menuName = "ScriptableObject/EffectPrefabs")]
public class EffectPrefabs : ScriptableObject
{
    public List<GameObject> Effects = new List<GameObject>();
}
