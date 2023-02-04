using System;
using UnityEngine;

[Serializable]
public class Interaction
{
    #region Datos

    public string missionCode;
    public string[] dialogs;
    public string character;
    public bool completed;
    public string missionToComplete;

    #endregion

    public Interaction(string missionCode, string[] dialogs, string character)
    {
        this.missionCode = missionCode;
        this.dialogs = dialogs;
        this.character = character;
        this.completed = false;
    }

    #region Metodos JSON

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static Interaction FromJson(string json)
    {
        return JsonUtility.FromJson<Interaction>(json);
    }

    #endregion
}
