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

    // Objects
    [SerializeField] private GameObject reward;

    // Dialogues
    [SerializeField] private Dialogue requestDialogue;
    [SerializeField] private Dialogue completedMissionDialogue;
    #endregion

    #region Propiedades
    public string Name { get => name; set => name = value; }
    public string Code { get => code; set => code = value; }
    public string Description { get => description; set => description = value; }
    public List<bool> Flags { get => flags; set => flags = value; }
    public List<Mission> Missions { get => missions; set => missions = value; }
    public GameObject Reward { get => reward; set => reward = value; }
    public Dialogue RequestDialogue { get => requestDialogue; set => requestDialogue = value; }
    public Dialogue CompletedMissionDialogue { get => completedMissionDialogue; set => completedMissionDialogue = value; }

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

[Serializable]
public class Dialogue
{

    [SerializeField] private string name; // Nombre del personaje
    [SerializeField] private string text;
    [SerializeField] private string image;
    [SerializeField] private bool side; // false: izquierda, true: derecha

    public Dialogue(string name, string text, string image, bool side)
    {
        this.name = name;
        this.text = text;
        this.image = image;
        this.side = side;
    }

    public string Name { get => name; set => name = value; }
    public string Text { get => text; set => text = value; }
    public string Image { get => image; set => image = value; }
    public bool Side { get => side; set => side = value; }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static Dialogue FromJson(string json)
    {
        return JsonUtility.FromJson<Dialogue>(json);
    }
}