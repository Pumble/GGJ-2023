using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Interaction
{
    #region Datos

    public int code;
    public string npc; // Nombre del NPC
    public int type; // 0: Default(Hablar), 1: Mission(Solicitar una mision), 2: Item(Dar un item), 3: Finalizar mision
    public List<string> dialogs;
    public string character;
    public bool completed;
    public List<int> requiredInteractions;
    public string missionCode; // Si el tipo es 0: parte de la mision, 1: codigo de la mision, 2: null, 3: codigo de la mision a terminar    
    public string itemToGive;

    #endregion

    #region Constructores

    public Interaction(int code, string npc, int type, List<string> dialogs, string character, bool completed, List<int> requiredInteractions, string missionCode, string itemToGive)
    {
        this.code = code;
        this.npc = npc;
        this.type = type;
        this.dialogs = dialogs;
        this.character = character;
        this.completed = completed;
        this.missionCode = missionCode;
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

public class InteractionWrapper
{
    public List<Interaction> interactions;

    public InteractionWrapper(List<Interaction> interactions)
    {
        this.interactions = interactions;
    }
}