using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string name;
    public float[] position = new float[2];
    public List<Mission> acceptedMissions;

    public PlayerData(Player p)
    {
        this.name = p.name;
        this.position[0] = p.transform.position.x;
        this.position[1] = p.transform.position.y;
        this.acceptedMissions = p.acceptedMissions;
    }
}
