using System.Collections.Generic;
using UnityEngine;

/**
 * Referencias:
 * Campos serializados: https://youtu.be/6CDzZeI4OX8
 */
[System.Serializable]
public class Mission
{
    #region Datos

    public string missionName;
    public string code;
    public string description;
    public bool completed;

    // Objetos
    public string rewardName;
    public string itemRequired;

    #endregion

    #region Metodos JSON

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static Mission FromJson(string json)
    {
        return JsonUtility.FromJson<Mission>(json);
    }

    #endregion
}

/**
 * Referencias:
 * JsonUtility no convierte arrays a JSON, necesita un wrapper
 * para que el JSON sea correcto
 * 
 * https://answers.unity.com/questions/1123326/jsonutility-array-not-supported.html
 * http://answers.unity.com/answers/1377926/view.html
 */
[System.Serializable]
public class MissionWrapper
{
    public List<Mission> missions;

    public MissionWrapper()
    {
        this.missions = new List<Mission>();
    }

    public MissionWrapper(List<Mission> ms)
    {
        this.missions = ms;
    }
}

[System.Serializable]
public class MissionData
{
    public string missionName;
    public string code;
    public string description;
    public bool completed;
    public string rewardName;
    public string itemRequired;

    public MissionData(Mission mission)
    {
        this.missionName = mission.missionName;
        this.code = mission.code;
        this.description = mission.description;
        this.completed = mission.completed;
        this.rewardName = mission.rewardName;
        this.itemRequired = mission.itemRequired;
    }
}