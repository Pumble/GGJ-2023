using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SavePlayerData(Player p)
    {
        PlayerData pd = new PlayerData(p);
        string dataPath = Application.persistentDataPath + "/player.save";
        FileStream fs = new FileStream(dataPath, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, pd);
        fs.Close();
        Debug.Log("Datos guardados");
    }

    public static PlayerData LoadPlayerData()
    {
        string dataPath = Application.persistentDataPath + "/player.save";
        if (File.Exists(dataPath))
        {
            FileStream fs = new FileStream(dataPath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            PlayerData pd = (PlayerData)bf.Deserialize(fs);
            fs.Close();
            Debug.Log("Datos cargados");
            return pd;
        }
        else
        {
            Debug.Log("No se encontro ningun archivo de guardado");
            return null;
        }
    }
}
