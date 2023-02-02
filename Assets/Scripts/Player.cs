using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Attributos
    private string name;

    #endregion

    #region Getters and Setters
    public string getName()
    {
        return name;
    }

    public void setName(string name)
    {
        this.name = name;
    }
    #endregion

    public void loadFromPlayerData(PlayerData pd)
    {
        this.name = pd.name;
        this.transform.position = new Vector2(pd.position[0], pd.position[1]);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
