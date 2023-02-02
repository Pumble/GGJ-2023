using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private GameManager()
    {
        // initialize your game manager here. Do not reference to GameObjects here (i.e. GameObject.Find etc.)
        // because the game manager will be created before the objects
    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }

            return instance;
        }
    }

    public GameObject Player;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveManager.SavePlayerData(Player.GetComponent<Player>());
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerData pd = SaveManager.LoadPlayerData();
            Player.GetComponent<Player>().loadFromPlayerData(pd);
        }
    }
}
