using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gametype { Normal, SpeedRun }

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Gametype gameType;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Sets the game type from our selections
    public void SetGameType(Gametype _gameType)
    {
        gameType = _gameType;
    }

    //To toggle between speedrun on or off
    public void ToggleSpeedRun(bool _speedRun)
    {
        if (_speedRun)
            SetGameType(Gametype.SpeedRun);
        else
            SetGameType(Gametype.Normal);
    }
}
