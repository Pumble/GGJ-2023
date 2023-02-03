[System.Serializable]
public class PlayerData
{
    public string name;
    public float[] position = new float[2];

    public PlayerData(Player p)
    {
        this.name = p.name;
        this.position[0] = p.transform.position.x;
        this.position[1] = p.transform.position.y;
    }
}
