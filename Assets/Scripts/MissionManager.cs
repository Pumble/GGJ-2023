using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;

public class MissionManager : MonoBehaviour
{
    #region Datos

    [SerializeField] private List<Mission> missions = new List<Mission>();
    public GameObject textMesh;
    private TextMeshProUGUI textMeshPro;
    private Player player;
    private int previousSize = 0;
    public GameObject missionModal;

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
        player = GameManager.Instance.Player.GetComponent<Player>();
        textMeshPro = textMesh.GetComponent<TextMeshProUGUI>();

        // TODO: REMOVER
        // PRUEBAS
        // Asignar esta mision al jugador
        //if (missions.Count > 0)
        //{
        //    StartCoroutine(addTestingMission());
        //}
    }

    private void LateUpdate()
    {            // Aqui actualizamos el panel de misiones
        if (previousSize != player.acceptedMissions.Count)
        {
            textMeshPro.text = "";
            foreach (Mission m in player.acceptedMissions)
            {
                textMeshPro.text += "<b>- <uppercase>" + m.missionName + "</uppercase></b>\n<line-indent=15%>" + m.description + "\n";
            }
        }
        previousSize = player.acceptedMissions.Count;
    }

    public void closeMissionModal()
    {
        missionModal.SetActive(false);
    }

    IEnumerator addTestingMission()
    {
        yield return new WaitForSeconds(10);
        if (GameManager.Instance.Player.GetComponent<Player>().acceptedMissions.Count == 0)
            GameManager.Instance.Player.GetComponent<Player>().AddMission(missions[0]);
    }
}
