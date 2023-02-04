using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

/**
 * Referencias
 * Pintar el gizmos: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html
 */

public class Player : MonoBehaviour
{
    #region Attributos

    public bool interacting = false;
    public List<Mission> acceptedMissions;
    public List<GameObject> inventory = new List<GameObject>();

    #endregion

    #region Metodos

    public void loadFromPlayerData(PlayerData pd)
    {
        this.name = pd.name;
        this.transform.position = new Vector2(pd.position[0], pd.position[1]);
        this.acceptedMissions = pd.acceptedMissions;
    }

    #endregion

    #region Verbos

    public void Interact(string missionCode, GameObject other)
    {
        if (other.tag == "NPC")
        {
            if (interacting == false)
            {
                interacting = true;
                /**
                 * Aqui lo que hacemos es detectar el NPC que tenemos de frente
                 * el codigo nos sirve para ver quien es y con ello filtramos las misiones
                 * 
                 * Si el codigo del NPC esta entre nuestras misiones, quiere decir que 
                 * debemos inciar una interactuacion entre ambos
                 * 
                 * Sino, pues debemos iniciar el dialogo por default
                 */
                Mission mission = this.acceptedMissions.FirstOrDefault(m => m.code == missionCode);
                NPC npc = other.GetComponent<NPC>();
                if (mission != null)
                {
                    // Ahora buscamos alguna interaccion en las misiones
                    Interaction interaction = mission.Interactions.FirstOrDefault(i => i.character == npc.name && i.completed == false);
                    if (interaction != null)
                    {
                        if (interaction.missionCode != null && interaction.missionCode != "")
                        {
                            npc.startDialogue(interaction);
                        }
                        else
                        {
                            /** Deberia ser una interaccion para completar
                             * Debemos revisar que mision completa y si esa mision tiene algun
                             * item por comprobar
                             */
                            if (interaction.missionToComplete != null)
                            {
                                Mission missionToComplete = this.acceptedMissions.FirstOrDefault(m => m.code == interaction.missionToComplete && m.completed == false);

                                bool check = true;
                                List<string> inventoryNames = this.inventory.ConvertAll(i => i.name.ToLower());
                                foreach (string item in missionToComplete.itemsToCheck)
                                {
                                    check = inventoryNames.Contains(item.ToLower()) && check;
                                }
                                if (check)
                                {
                                    // Hay que remover los items del array
                                    foreach (string item in missionToComplete.itemsToCheck)
                                    {
                                        this.inventory.Remove(this.inventory.FirstOrDefault(i => i.name.ToLower() == item.ToLower()));
                                    }
                                    npc.startDialogueToFinish(interaction, missionToComplete);
                                }
                                else
                                {
                                    npc.startDefaultDialogue();
                                    Debug.Log("Mision detectada, interaccion para completar encontrada pero sin los items requeridos");
                                }
                            }
                        }
                    }
                    else
                    {
                        // Iniciar dialogo por defecto
                        npc.startDefaultDialogue();
                        Debug.Log("Mision detectada, pero no hay interacciones");
                    }
                }
                else
                {
                    // Iniciar dialogo por defecto
                    npc.startDefaultDialogue();
                }
            }
        }
    }

    public void AddMission(Mission m)
    {
        this.acceptedMissions.Add(m);
        MissionManager.Instance.Missions.Remove(m);
        Debug.Log("Mision aceptada: " + m.missionName);

        m.StartMission();
    }

    public void AddReward(string reward)
    {
        GameObject[] rewards = GameObject.FindGameObjectsWithTag("Reward");
        GameObject objectReward = rewards.FirstOrDefault(r => r.name == reward);
        this.inventory.Add(objectReward);

        DialogData dialogData = new DialogData("/sound:itemCollected/Item obtenido: " + this.name + "/click//close/", "Player");
        GameManager.Instance.DialogManager.Show(dialogData);
    }

    #endregion

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, 2f);
    }
}
