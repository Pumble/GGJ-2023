using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadJsonData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveManager.LoadInGameData();
    }
}
