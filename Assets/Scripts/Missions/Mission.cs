using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Referencias:
 * Campos serializados: https://youtu.be/6CDzZeI4OX8
 */
[Serializable]
public class Mission
{
    #region Datos
    
    [SerializeField] private string name;
    [SerializeField] private string code;
    [SerializeField] private string description;
    [SerializeField] private List<bool> flags;
    [SerializeField] private List<Mission> missions;

    // Objetos
    [SerializeField] private GameObject reward;

    // Dialogos
    [SerializeField] private string requestDialogue;
    [SerializeField] private string completedMissionDialogue;
    
    #endregion
    
    #region Propiedades
    
    public string Name { get => name; set => name = value; }
    public string Code { get => code; set => code = value; }
    public string Description { get => description; set => description = value; }
    public List<bool> Flags { get => flags; set => flags = value; }
    public List<Mission> Missions { get => missions; set => missions = value; }
    public GameObject Reward { get => reward; set => reward = value; }
    public string RequestDialogue { get => requestDialogue; set => requestDialogue = value; }
    public string CompletedMissionDialogue { get => completedMissionDialogue; set => completedMissionDialogue = value; }

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

    public virtual bool missionCompleted()
    {
        // check if all objectives are completed
        List<bool> completedFlags = flags.FindAll(f => f == true);
        return completedFlags.Count == flags.Count;
    }
}