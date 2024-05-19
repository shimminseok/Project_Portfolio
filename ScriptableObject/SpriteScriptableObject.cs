using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite", menuName = "ScriptableObjects/Sprite")]

public class SpriteScriptableObject : ScriptableObject
{
    public List<Sprite> spriteList;

    public Sprite GetSprite(string _name)
    {
        return spriteList.Find(x => x.name == _name);
    }
}
