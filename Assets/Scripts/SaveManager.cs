using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveManager
{
    //#region PlayerData

    //public static void SavePlayerData(Player p)
    //{
    //    PlayerData pd = new PlayerData(p);
    //    string dataPath = Application.persistentDataPath + "/player.save";
    //    FileStream fs = new FileStream(dataPath, FileMode.Create);
    //    BinaryFormatter bf = new BinaryFormatter();
    //    bf.Serialize(fs, pd);
    //    fs.Close();
    //    Debug.Log("Datos guardados");
    //}

    //public static PlayerData LoadPlayerData()
    //{
    //    string dataPath = Application.persistentDataPath + "/player.save";
    //    if (File.Exists(dataPath))
    //    {
    //        FileStream fs = new FileStream(dataPath, FileMode.Open);
    //        BinaryFormatter bf = new BinaryFormatter();
    //        PlayerData pd = (PlayerData)bf.Deserialize(fs);
    //        fs.Close();
    //        Debug.Log("Datos cargados");
    //        return pd;
    //    }
    //    else
    //    {
    //        Debug.Log("No se encontro ningun archivo de guardado");
    //        return null;
    //    }
    //}

    //#endregion

    #region Misiones

    public static void SaveMissionsData(MissionWrapper wrapper)
    {
        string json = JsonUtility.ToJson(wrapper, true);
        string path = Application.persistentDataPath + "/missions.json";
        File.WriteAllText(path, json);
        Debug.Log("Misiones guardadas");
    }

    public static List<Mission> LoadMissionsData()
    {
        string dataPath = Application.persistentDataPath + "/missions.json";
        if (File.Exists(dataPath))
        {
            MissionWrapper wrapper = JsonUtility.FromJson<MissionWrapper>(File.ReadAllText(dataPath));
            Debug.Log("Misiones cargadas");
            return wrapper.missions;
        }
        else
        {
            Debug.Log("No se encontro ningun archivo de misiones");
            return null;
        }

    }

    #endregion

    #region Informacion del jugador

    public static void SavePlayerData(PlayerData wrapper)
    {
        string json = JsonUtility.ToJson(wrapper, true);
        string path = Application.persistentDataPath + "/player.json";
        File.WriteAllText(path, json);
        Debug.Log("Informacion del jugador guardada");
    }

    public static PlayerData LoadPlayerData()
    {
        string dataPath = Application.persistentDataPath + "/player.json";
        if (File.Exists(dataPath))
        {
            PlayerData wrapper = JsonUtility.FromJson<PlayerData>(File.ReadAllText(dataPath));
            Debug.Log("Informacion del jugador cargada");
            return wrapper;
        }
        else
        {
            Debug.Log("No se encontro ningun archivo de Informacion del jugador");
            return null;
        }
    }

    #endregion
}
