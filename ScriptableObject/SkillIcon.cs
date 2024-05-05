using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillIcon", menuName = "ScriptableObjects/SkillIcon")]

public class SkillIcon : ScriptableObject
{
    public List<Sprite> spriteList;

    public Sprite GetSprite(string _name)
    {
        return spriteList.Find(x => x.name == _name);
    }
}
