using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using System.Linq;
using static UnityEditor.Progress;
using System.Text;
using System;
using UnityEngine.XR;
using UnityEngine.TextCore.Text;

public class NPCData
{
    #region Datos

    public string name;
    public string defaultDialogue;
    public List<Interaction> interactions;

    #endregion

    #region Constructores

    public NPCData(string name, string defaultDialogue, List<Interaction> interactions)
    {
        this.name = name;
        this.defaultDialogue = defaultDialogue;
        this.interactions = interactions;
    }

    public NPCData(NPC npc)
    {
        this.name = npc.name;
        this.defaultDialogue = npc.defaultDialogue;
        this.interactions = npc.interactions;
    }

    #endregion

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

/**
 * Referencias:
 * Documentacion DDialog: https://github.com/DoublSB/UnityDialogAsset
 */
public class NPC : MonoBehaviour
{
    #region Variables

    public string defaultDialogue;
    public List<Interaction> interactions;

    private Player player;

    #endregion

    #region Constructores

    public NPC(string name, string defaultDialogue, List<Interaction> interactions)
    {
        this.name = name;
        this.defaultDialogue = defaultDialogue;
        this.interactions = interactions;
    }

    public NPC(NPCData data)
    {
        this.name = data.name;
        this.defaultDialogue = data.defaultDialogue;
        this.interactions = data.interactions;
    }

    #endregion

    private void Start()
    {
        player = GameManager.Instance.Player.GetComponent<Player>();
    }

    #region Triggers

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            if (player.interacting == false)
            {
                // Revisar si este NPC tiene alguna interaccion con el jugador
                if (interactions.Count > 0)
                {
                    /** 
                     * Debemos filtrar las interacciones que:
                     * 1- Que no esten completas(osea que no se han iniciado
                     * 2- Cuyos requisitos esten completados
                     */
                    List<Interaction> aviableInteractions = new List<Interaction>();
                    foreach (Interaction i in this.interactions)
                    {
                        if (i.completed == false)
                        {
                            if (i.requiredInteraction != -1)
                            {
                                Interaction requiredInteraction = GameManager.Instance.interactions.FirstOrDefault(j => j.code == i.requiredInteraction);
                                if (requiredInteraction != null)
                                {
                                    if (requiredInteraction.completed)
                                    {
                                        aviableInteractions.Add(i);
                                    }
                                    else
                                    {
                                        Debug.Log("La interaccion(" + i.code + ") requiere a: " + i.requiredInteraction + " aun no esta completada");
                                    }
                                }
                                else
                                {
                                    Debug.Log("La interaccion(" + i.code + ") requiere a: " + i.requiredInteraction + " y no se encontro en la lista");
                                }
                            }
                            else
                            {
                                aviableInteractions.Add(i);
                            }
                        }
                    }

                    if (aviableInteractions.Count > 0)
                    {
                        // Resolvamos las interacciones de 1 en 1, asi que solo le vamos a pasar la primera
                        // la clase Player se encargara
                        player.Interact(aviableInteractions[0], this);
                    }
                    else
                    {
                        // Ya agoto las interacciones con este personaje
                        this.startDefaultDialogue();
                    }
                }
                else
                {
                    this.startDefaultDialogue();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.Player.GetComponent<Player>().interacting = false;
        }
    }

    #endregion

    #region Dialogos

    public void startDefaultDialogue()
    {
        // Aqui iniciamos el dialogo por default
        byte[] bytes = Encoding.Default.GetBytes("/emote:Normal/" + this.defaultDialogue);
        DialogData dialogData = new DialogData(Encoding.UTF8.GetString(bytes), this.name);
        GameManager.Instance.DialogManager.Show(dialogData);

        dialogData.Callback = () =>
        {
            GameManager.Instance.Player.GetComponent<Player>().interacting = false;
        };
    }

    #endregion

    #region Verbos

    public void Talk(Interaction interaction)
    {
        List<DialogData> dialogTexts = new List<DialogData>();
        string last = interaction.dialogs.Last();

        foreach (string d in interaction.dialogs)
        {
            if (d != last)
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character));
            }
            else
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character, () =>
                {
                    interaction.completed = true;
                    player.interacting = false;
                }));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }

    public void RequestAMission(Interaction interaction)
    {
        List<DialogData> dialogTexts = new List<DialogData>();
        string last = interaction.dialogs.Last();

        foreach (string d in interaction.dialogs)
        {
            if (d != last)
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character));
            }
            else
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character, () =>
                {
                    interaction.completed = true;
                    Mission m = MissionManager.Instance.Missions.FirstOrDefault(m => m.code == interaction.missionCode && m.completed == false);
                    if (m != null)
                    {
                        player.AddMission(m);
                    }
                    else
                    {
                        Debug.LogError("No se encontro la mision: " + interaction.missionCode + " para asignar al jugador");
                    }
                    player.interacting = false;
                }));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }

    public void GiveAItem(Interaction interaction)
    {
        List<DialogData> dialogTexts = new List<DialogData>();
        string last = interaction.dialogs.Last();

        foreach (string d in interaction.dialogs)
        {
            if (d != last)
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character));
            }
            else
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character, () =>
                {
                    interaction.completed = true;
                    player.AddReward(interaction.itemToGive);
                    player.interacting = false;
                }));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }

    public void FinishAMission(Interaction interaction)
    {
        List<DialogData> dialogTexts = new List<DialogData>();

        Mission mission = player.acceptedMissions.FirstOrDefault(m => m.code == interaction.missionCode);
        interaction.dialogs.Add("Mision '" + mission.missionName + "' completada/click//close/");
        string last = interaction.dialogs.Last();

        foreach (string d in interaction.dialogs)
        {
            if (d != last)
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character));
            }
            else
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character, () =>
                {
                    interaction.completed = true;
                    player.FinishMission(mission);
                    player.interacting = false;
                }));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }

    #endregion
}