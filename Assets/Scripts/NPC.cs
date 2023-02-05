using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using System.Linq;
using static UnityEditor.Progress;

/**
 * Referencias:
 * Documentacion DDialog: https://github.com/DoublSB/UnityDialogAsset
 */
public class NPC : MonoBehaviour
{
    #region Variables

    [SerializeField] private string code;
    [SerializeField] private string defaultDialogue;
    [SerializeField] private string missionRequest;

    #endregion

    #region Propiedades

    public string Code { get => code; set => code = value; }
    public string DefaultDialogue { get => defaultDialogue; set => defaultDialogue = value; }
    public string MissionRequest { get => missionRequest; set => missionRequest = value; }

    #endregion

    #region Triggers

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            Player player = GameManager.Instance.Player.GetComponent<Player>();
            if (this.missionRequest == null || this.missionRequest == "")
            {
                // Iniciamos la interaccion
                player.Interact(this.Code, this.transform.gameObject);
            }
            else
            {
                // Ya acepto la mision?
                Mission mission = player.acceptedMissions.FirstOrDefault(m => m.code == this.missionRequest);
                if (mission == null) // No tiene la mision
                {
                    Mission toAccept = MissionManager.Instance.Missions.FirstOrDefault(m => m.code == this.missionRequest);
                    if (toAccept != null)
                    {
                        player.AddMission(toAccept);
                    }
                }
                else {
                    // Ya tiene la mision, hay que atender con una interaccion
                    player.Interact(this.Code, this.transform.gameObject);
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

    public void startDefaultDialogue()
    {
        // Aqui iniciamos el dialogo por default
        DialogData dialogData = new DialogData(this.DefaultDialogue, this.Code);
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
        string last = interaction.dialogs[interaction.dialogs.Length - 1];
        foreach (string d in interaction.dialogs)
        {
            if (d.Equals(last))
            {
                dialogTexts.Add(new DialogData(d, interaction.character, () =>
                {
                    GameManager.Instance.Player.GetComponent<Player>().interacting = false;
                    interaction.completed = true;

                    if (interaction.missionCode != null && interaction.missionCode != "")
                    {
                        Mission mission = MissionManager.Instance.Missions.FirstOrDefault(m => m.code == interaction.missionCode);
                        GameManager.Instance.Player.GetComponent<Player>().AddMission(mission);
                    }
                }));
            }
            else
            {
                dialogTexts.Add(new DialogData(d, interaction.character));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }

    public void startDialogueToFinish(Interaction interaction, Mission mission)
    {
        // Aqui iniciamos el dialogo por default
        List<DialogData> dialogTexts = new List<DialogData>();
        string last = interaction.dialogs[interaction.dialogs.Length - 1];
        foreach (string d in interaction.dialogs)
        {
            if (d.Equals(last))
            {
                dialogTexts.Add(new DialogData(d, interaction.character, () =>
                {
                    Player player = GameManager.Instance.Player.GetComponent<Player>();

                    player.interacting = false;
                    interaction.completed = true;

                    if (interaction.missionToComplete != null && interaction.missionToComplete != "")
                    {
                        mission.completed = true;
                        Debug.Log("Mision completada: " + mission.code);
                        if (mission.rewardName != null && mission.rewardName != "")
                        {
                            player.AddReward(mission.rewardName);
                        }
                        player.acceptedMissions.Remove(mission);
                    }
                }));
            }
            else
            {
                dialogTexts.Add(new DialogData(d, interaction.character));
            }
        }
        GameManager.Instance.DialogManager.Show(dialogTexts);
    }
}