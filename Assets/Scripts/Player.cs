using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //public void Interact(string missionCode, GameObject other)
    //{
    //    if (other.tag == "NPC")
    //    {
    //        if (interacting == false)
    //        {
    //            interacting = true;

    //            /**
    //             * Aqui lo que hacemos es detectar el NPC que tenemos de frente
    //             * el codigo nos sirve para ver quien es y con ello filtramos las misiones
    //             * 
    //             * Si el codigo del NPC esta entre nuestras misiones, quiere decir que 
    //             * debemos inciar una interactuacion entre ambos
    //             * 
    //             * Sino, pues debemos iniciar el dialogo por default
    //             */
    //            Mission mission = this.acceptedMissions.FirstOrDefault(m =>
    //            {
    //                return m.code == missionCode && m.completed == false;
    //            });
    //            NPC npc = other.GetComponent<NPC>();
    //            if (mission != null)
    //            {
    //                // Ahora buscamos alguna interaccion en las misiones
    //                Interaction interaction = mission.Interactions.FirstOrDefault(i => i.character == npc.name && i.completed == false);
    //                if (interaction != null)
    //                {
    //                    if (interaction.missionCode != null && interaction.missionCode != "")
    //                    {
    //                        npc.startDialogue(interaction);
    //                    }
    //                    else
    //                    {
    //                        /** Deberia ser una interaccion para completar
    //                         * Debemos revisar que mision completa y si esa mision tiene algun
    //                         * item por comprobar
    //                         */
    //                        if (interaction.missionToComplete != null)
    //                        {
    //                            Mission missionToComplete = this.acceptedMissions.FirstOrDefault(m => m.code == interaction.missionToComplete && m.completed == false);

    //                            bool check = true;
    //                            List<string> inventoryNames = this.inventory.ConvertAll(i => i.name.ToLower());
    //                            foreach (string item in missionToComplete.itemsToCheck)
    //                            {
    //                                check = inventoryNames.Contains(item.ToLower()) && check;
    //                            }
    //                            if (check)
    //                            {
    //                                // Hay que remover los items del array
    //                                foreach (string item in missionToComplete.itemsToCheck)
    //                                {
    //                                    this.inventory.Remove(this.inventory.FirstOrDefault(i => i.name.ToLower() == item.ToLower()));
    //                                }
    //                                npc.startDialogueToFinish(interaction, missionToComplete);
    //                            }
    //                            else
    //                            {
    //                                npc.startDefaultDialogue();
    //                                Debug.Log("Mision detectada, interaccion para completar encontrada pero sin los items requeridos");
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    // Iniciar dialogo por defecto
    //                    npc.startDefaultDialogue();
    //                    Debug.Log("Mision detectada, pero no hay interacciones");
    //                }
    //            }
    //            else
    //            {
    //                // Iniciar dialogo por defecto
    //                npc.startDefaultDialogue();
    //            }
    //        }
    //    }
    //}

    public void AddMission(Mission m)
    {
        this.acceptedMissions.Add(m);
        MissionManager.Instance.Missions.Remove(m);
        Debug.Log("Mision aceptada: " + m.missionName);

        DialogData dialogData = new DialogData("/sound:missionAdded/Misión aceptada: " + m.missionName + "/click//close/", "Player");
        GameManager.Instance.DialogManager.Show(dialogData);
    }

    public void FinishMission(Mission m)
    {
        // Quitar los objetos requeridos
        if (m.itemRequired != null)
        {
            GameObject[] rewards = GameObject.FindGameObjectsWithTag("Reward");
            GameObject objectReward = rewards.FirstOrDefault(r => r.name == m.itemRequired);
            this.inventory.Remove(objectReward);
        }

        m.completed = true;
        Debug.Log("Mision: " + m.code + " completada!");

        DialogData dialogData = new DialogData("/emote:Normal//sound:missionCompleted/¡Misión: " + m.missionName + " completada!/click//close/", "Player");
        GameManager.Instance.DialogManager.Show(dialogData);
        RewardAndAddRemoveMission(m);
    }

    public void AddReward(string reward, bool notify = true)
    {
        GameObject[] rewards = GameObject.FindGameObjectsWithTag("Reward");
        GameObject objectReward = rewards.FirstOrDefault(r => r.name == reward);
        this.inventory.Add(objectReward);

        if (notify)
        {
            DialogData dialogData = new DialogData("/emote:Normal//sound:itemCollected/Item obtenido: " + objectReward.name + "/click//close/", "Player");
            GameManager.Instance.DialogManager.Show(dialogData);
        }
    }

    IEnumerator RewardAndAddRemoveMission(Mission m)
    {
        yield return new WaitForSeconds(2);
        this.acceptedMissions.Remove(m);
        MissionManager.Instance.Missions.Remove(m);
        this.AddReward(m.rewardName);
    }


    public void Interact(Interaction interaction, NPC npc)
    {
        if (interacting == false)
        {
            interacting = true;

            if (interaction.completed)
            {
                npc.startDefaultDialogue();
                Debug.Log(interaction);
                Debug.LogError("Me pasaron una interaccion completada");
            }
            else
            {
                switch (interaction.type)
                {
                    case 0: // Solo hablar
                        npc.Talk(interaction);
                        break;
                    case 1: // Nueva mision
                        npc.RequestAMission(interaction);
                        break;
                    case 2: // Item por recibir
                        npc.GiveAItem(interaction);
                        break;
                    case 3: // Finalizar mision
                        npc.FinishAMission(interaction);
                        break;
                    default:
                        npc.startDefaultDialogue();
                        break;
                }
            }
        }
    }

    #endregion

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, 2f);
    }
}
