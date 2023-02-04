using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    #region Datos

    [SerializeField] private List<Mission> missions = new List<Mission>();

    #endregion

    #region Propiedades

    public static MissionManager Instance { get; private set; }
    public List<Mission> Missions { get => missions; set => missions = value; }

    #endregion

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        missions = SaveManager.LoadMissionsData();

        // TODO: REMOVER
        // PRUEBAS
        // Asignar esta mision al jugador
        if (missions.Count > 0)
        {
            StartCoroutine(addTestingMission());
        }
    }

    IEnumerator addTestingMission()
    {
        yield return new WaitForSeconds(10);
        if (GameManager.Instance.Player.GetComponent<Player>().acceptedMissions.Count == 0)
            GameManager.Instance.Player.GetComponent<Player>().AddMission(missions[0]);
    }
}
