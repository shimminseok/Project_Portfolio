using System.Collections;
using System.Collections.Generic;
using Tables;
using UnityEngine;
using UnityEngine.Jobs;

public class GameManager : Singleton<GameManager>
{
    public JoystickController joystickController;
    float gameSpeed = 1;

    public int stageStep;

    public bool isAuto = true;


    public float GameSpeed { get { return gameSpeed; } }
    public void BossChallenge()
    {

    }
    public void SetGameSpeed(float _speed)
    {
        gameSpeed = _speed;
    }


}
