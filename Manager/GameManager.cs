using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public JoystickController joystickController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateCharacterPrefab(string _prefabName,Transform _root)
    {
        string path = "Prefabs/Character/" + _prefabName;
        GameObject go = Instantiate(Resources.Load<GameObject>(path),_root);
        return go;
    }
}
