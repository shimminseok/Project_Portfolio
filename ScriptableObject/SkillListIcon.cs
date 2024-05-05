using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillListIcon", menuName = "ScriptableObjects/SkillListIcon")]

public class SkillListIcon : ScriptableObject
{
    public List<Sprite> spriteList;

    public Sprite GetSprite(string _name)
    {
        return spriteList.Find(x => x.name == _name);
    }
}
