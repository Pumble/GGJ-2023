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
    public int type; // 0: Default(Hablar), 1: Mission(Solicitar una mision), 2: Item(Dar un item), 3: Finalizar mision
    public string itemToGive;

    #endregion

    #region Constructores

    public Interaction(string missionCode, string[] dialogs, string character)
    {
        this.missionCode = missionCode;
        this.dialogs = dialogs;
        this.character = character;
        this.completed = false;
    }

    public Interaction(string missionCode, string[] dialogs, string character, bool completed, string missionToComplete, int type, string itemToGive) : this(missionCode, dialogs, character)
    {
        this.completed = completed;
        this.missionToComplete = missionToComplete;
        this.type = type;
        this.itemToGive = itemToGive;
    }

    #endregion  |   

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
