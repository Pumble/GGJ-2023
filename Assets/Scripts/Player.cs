using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Referencias
 * Pintar el gizmos: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html
 */

public class Player : MonoBehaviour
{
    #region Attributos
    [SerializeField] private bool interacting = false;
    [SerializeField] private List<Mission> acceptedMissions;
    #endregion

    #region Getters and Setters
    public List<Mission> AcceptedMissions { get => acceptedMissions; set => acceptedMissions = value; }
    public bool Interacting { get => interacting; set => interacting = value; }
    #endregion

    #region Metodos
    public void loadFromPlayerData(PlayerData pd)
    {
        this.name = pd.name;
        this.transform.position = new Vector2(pd.position[0], pd.position[1]);
    }
    #endregion

    #region Verbos
    public void Interact(string code, GameObject other)
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
                List<Mission> missionsWithThisNPC = this.AcceptedMissions.FindAll(m => m.Code == code);
                if (missionsWithThisNPC.Count > 0)
                {
                    // Iniciar dialogo de mision
                }
                else
                {
                    // Iniciar dialogo por defecto
                    other.GetComponent<NPC>().startDefaultDialogue();
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
