using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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
            byte[] bytes = Encoding.Default.GetBytes(File.ReadAllText(dataPath));
            MissionWrapper wrapper = JsonUtility.FromJson<MissionWrapper>(Encoding.UTF8.GetString(bytes));
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
            byte[] bytes = Encoding.Default.GetBytes(File.ReadAllText(dataPath));
            PlayerData wrapper = JsonUtility.FromJson<PlayerData>(Encoding.UTF8.GetString(bytes));
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

    #region Interacciones

    public static void SaveInteractionsData(InteractionWrapper wrapper)
    {
        string json = JsonUtility.ToJson(wrapper, true);
        string path = Application.persistentDataPath + "/interactions.json";
        File.WriteAllText(path, json);
        Debug.Log("Interacciones guardadas");
    }

    public static List<Interaction> LoadInteractionsData()
    {
        string dataPath = Application.persistentDataPath + "/interactions.json";
        if (File.Exists(dataPath))
        {
            byte[] bytes = Encoding.Default.GetBytes(File.ReadAllText(dataPath));
            InteractionWrapper wrapper = JsonUtility.FromJson<InteractionWrapper>(Encoding.UTF8.GetString(bytes));
            Debug.Log("Interacciones cargadas");
            return wrapper.interactions;
        }
        else
        {
            Debug.Log("No se encontro ningun archivo de interacciones");
            return null;
        }
    }

    #endregion

    #region In game

    public static void SaveInGameData(PlayerData playerWrapper, MissionWrapper missionWrapper, InteractionWrapper interactionWrapper)
    {
        // Jugador
        string json = JsonUtility.ToJson(playerWrapper, true);
        string path = Application.persistentDataPath + "/in-game-player.json";
        File.WriteAllText(path, json);
        Debug.Log("Informacion del jugador in-game guardada");

        // Misiones
        string json2 = JsonUtility.ToJson(missionWrapper, true);
        string path2 = Application.persistentDataPath + "/in-game-missions.json";
        File.WriteAllText(path2, json2);
        Debug.Log("Misiones in-game guardadas");

        string json3 = JsonUtility.ToJson(interactionWrapper, true);
        string path3 = Application.persistentDataPath + "/in-game-interactions.json";
        File.WriteAllText(path3, json3);
        Debug.Log("Interacciones in-game guardadas");
    }

    public static void LoadInGameData()
    {
        // PLAYER
        string dataPath = Application.persistentDataPath + "/in-game-player.json";
        if (File.Exists(dataPath))
        {
            byte[] bytes = Encoding.Default.GetBytes(File.ReadAllText(dataPath));
            PlayerData pd = JsonUtility.FromJson<PlayerData>(Encoding.UTF8.GetString(bytes));
            GameManager.Instance.Player.GetComponent<Player>().loadFromPlayerData(pd);
            Debug.Log("Informacion del jugador in-game cargada");
        }
        else
        {
            Debug.Log("No se encontro ningun archivo de Informacion in-game del jugador");
        }

        string dataPath2 = Application.persistentDataPath + "/in-game-missions.json";
        if (File.Exists(dataPath2))
        {
            byte[] bytes = Encoding.Default.GetBytes(File.ReadAllText(dataPath2));
            MissionWrapper mw= JsonUtility.FromJson<MissionWrapper>(Encoding.UTF8.GetString(bytes));
            Debug.Log("Misiones in-game cargadas");
            MissionManager.Instance.Missions = mw.missions;
        }
        else
        {
            Debug.Log("No se encontro ningun archivo in-game de misiones");
        }

        string dataPath3 = Application.persistentDataPath + "/in-game-interactions.json";
        if (File.Exists(dataPath3))
        {
            byte[] bytes = Encoding.Default.GetBytes(File.ReadAllText(dataPath3));
            InteractionWrapper iw = JsonUtility.FromJson<InteractionWrapper>(Encoding.UTF8.GetString(bytes));
            Debug.Log("Interacciones in-game cargadas");
            GameManager.Instance.interactions = iw.interactions;

            // Distribuir las interacciones
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            foreach (GameObject npc in npcs)
            {
                NPC npcScript = npc.GetComponent<NPC>();
                npcScript.interactions = new List<Interaction>();
                foreach (Interaction interaction in GameManager.Instance.interactions)
                {
                    if (interaction.character == npc.name)
                    {
                        npcScript.interactions.Add(interaction);
                    }
                }
            }
        }
        else
        {
            Debug.Log("No se encontro ningun archivo de interacciones in-game");
        }
    }

    #endregion
}
