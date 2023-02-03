using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Attributos
    private string name;
    private List<Mission> acceptedMissions;
    #endregion

    #region Getters and Setters
    public string Name { get => name; set => name = value; }
    public List<Mission> AcceptedMissions { get => acceptedMissions; set => acceptedMissions = value; }
    #endregion

    public void loadFromPlayerData(PlayerData pd)
    {
        this.Name = pd.name;
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

    #region Verbos
    public void Talk()
    {

    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "NPC")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Do something else here");
        }
    }
    #endregion
}
