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
            // Revisar si este NPC tiene alguna interaccion con el jugador
            if (interactions.Count > 0)
            {
                List<Interaction> aviableInteractions = this.interactions.FindAll(i => i.completed == false);
            }
            else
            {
                this.startDefaultDialogue();
            }




            // Nos esta solicitando una mision?
            if (this.missionRequest != null && this.missionRequest != "")
            {
                // Aseguramos que la mision no este ya aceptada
                Mission mission = player.acceptedMissions.FirstOrDefault(m => m.code == this.missionRequest);
                if (mission == null) // No tiene la mision
                {
                    Mission toAccept = MissionManager.Instance.Missions.FirstOrDefault(m => m.code == this.missionRequest);
                    if (toAccept != null)
                    {
                        player.AddMission(toAccept);
                    }
                    else
                    {
                        // A falta de algo, mejor interactuamos
                        player.Interact(this.Code, this.transform.gameObject);
                    }
                }
                else
                {
                    // Ya tiene la mision, hay que atender con una interaccion
                    player.Interact(this.Code, this.transform.gameObject);
                }
            }
            else
            {
                player.Interact(this.Code, this.transform.gameObject);
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

    public void startDialogue(Interaction interaction)
    {
        // Aqui iniciamos el dialogo por default
        List<DialogData> dialogTexts = new List<DialogData>();
        // Obtenemos todos los items menos el ultimo
        string last = interaction.dialogs.Last();

        foreach (string d in interaction.dialogs)
        {
            if (d != last)
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character));
            }
            else
            {
                dialogTexts.Add(new DialogData("/emote:Normal/" + d, interaction.character, () => this.starInteraction(interaction)));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }

    public void startDialogueToFinish(Interaction interaction, Mission mission)
    {
        DialogData dialogData = new DialogData("/emote:Normal/" + mission.completedMissionDialogue, interaction.character, () =>
        {
            Player player = GameManager.Instance.Player.GetComponent<Player>();

            player.interacting = false;
            interaction.completed = true;
            mission.completed = true;
            Debug.Log("Mision completada: " + mission.code);

            if (mission.rewardName != null && mission.rewardName != "")
            {
                player.AddReward(mission.rewardName);
            }
            player.acceptedMissions.Remove(mission);
        });
        GameManager.Instance.DialogManager.Show(dialogData);
    }

    private void starInteraction(Interaction interaction)
    {
        // Siempre que la interaccion este abierta
        if (!interaction.completed)
        {
            switch (interaction.type)
            {
                case 1: // Nueva mision
                    Mission m = MissionManager.Instance.Missions.FirstOrDefault(m => m.code == interaction.missionCode);
                    if (m != null)
                    {
                        player.AddMission(m);
                    }
                    else
                    {
                        Debug.Log(m);
                        Debug.LogError("interactionCallback: Se esperaba una nueva mision x interaccion, pero no encontramos la mision");
                    }
                    break;
                case 2: // Item por recibir
                    player.AddReward(interaction.itemToGive);
                    break;
                case 3: // Finalizar mision
                    Mission m2 = player.acceptedMissions.FirstOrDefault(m => m.code == interaction.missionCode);
                    if (m2 != null)
                    {
                        m2.completed = true;
                        this.startDialogueToFinish(interaction, m2);
                    }
                    else
                    {
                        Debug.Log(m2);
                        Debug.LogError("interactionCallback: Se esperaba una finalizar una mision x interaccion, pero no encontramos la mision");
                    }
                    break;
                default:
                    break;
            }
            interaction.completed = true;
        }
        GameManager.Instance.Player.GetComponent<Player>().interacting = false;
    }

    #endregion
}