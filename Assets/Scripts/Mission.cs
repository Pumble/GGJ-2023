using Doublsb.Dialog;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
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
    public string parentCode; // Si es nulo: es padre o raiz, si no: es submision
    public bool completed;

    // Objetos
    public string rewardName;

    // Dialogos
    public string requestDialogue;
    public string completedMissionDialogue;
    private List<Interaction> interactions = new List<Interaction>();
    public Interaction[] interactionsArray;

    // Items por checkear
    public List<string> itemsToCheck = new List<string>();

    #endregion

    #region Propiedades

    public List<Interaction> Interactions
    {
        get
        {
            interactions = new List<Interaction>(interactionsArray);
            return interactions;
        }
        set
        {
            interactions = value;
            interactionsArray = interactions.ToArray();
        }
    }

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

    public void StartMission()
    {
        byte[] bytes = Encoding.Default.GetBytes("/sound:missionAdded/Nueva mision: " + this.requestDialogue);
        DialogData dialogData = new DialogData(Encoding.UTF8.GetString(bytes), "Player");
        GameManager.Instance.DialogManager.Show(dialogData);
    }

    public void FinishMission()
    {
        DialogData dialogData = new DialogData(this.completedMissionDialogue, "Player", () =>
        {
            Player p = GameManager.Instance.Player.GetComponent<Player>();
            if (this.rewardName != null && this.rewardName != "")
                p.AddReward(this.rewardName);
            p.acceptedMissions.Remove(this);
        });
        GameManager.Instance.DialogManager.Show(dialogData);
    }

    public List<Interaction> GetInteractions(string code)
    {
        return interactions.FindAll(i => i.missionCode == code);
    }
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
    public string parentCode; // Si es nulo: es padre o raiz, si no: es submision
    public bool completed;
    public MissionData[] subMissionsArray;
    public string rewardName;
    public string requestDialogue;
    public string completedMissionDialogue;
    public Interaction[] interactionsArray;
    public List<string> itemsToCheck = new List<string>();

    public MissionData(Mission mission)
    {
        this.missionName = mission.missionName;
        this.code = mission.code;
        this.description = mission.description;
        this.parentCode = mission.parentCode;
        this.completed = mission.completed;

        this.rewardName = mission.rewardName;
        this.requestDialogue = mission.requestDialogue;
        this.completedMissionDialogue = mission.completedMissionDialogue;
        this.itemsToCheck = mission.itemsToCheck;
        //this.interactionsArray = mission.Interactions.ToArray();

        //this.subMissionsArray = new MissionData[mission.SubMissions.Count];
        //for (int i = 0; i < mission.SubMissions.Count; i++)
        //{
        //    this.subMissionsArray[i] = new MissionData(mission.SubMissions[i]);
        //}
    }
}