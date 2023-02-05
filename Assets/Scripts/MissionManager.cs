using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    #region Datos

    [SerializeField] private List<Mission> missions = new List<Mission>();
    public GameObject textMesh;
    private TextMeshProUGUI textMeshPro;
    private Player player;
    private int previousSize = 0;
    public GameObject missionModal;

    public GameObject itemsModal;
    public GameObject textMeshItems;
    private TextMeshProUGUI textMeshProItems;
    private int previousSizeItems = 0;

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
        textMeshProItems = textMeshItems.GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        // Aqui actualizamos el panel de misiones

        textMeshPro.text = "";
        foreach (Mission m in player.acceptedMissions)
        {
            textMeshPro.text += "<line-indent=0%><b>- <uppercase>" + m.missionName + "</uppercase></b></line-indent>\n<line-indent=15%>" + m.description + "</line-indent>\n";
        }

        // Aqui actualizamos el panel de misiones

        textMeshProItems.text = "";
        foreach (GameObject item in player.inventory)
        {
            textMeshProItems.text += "<line-indent=0%><b>- " + item.name + "</line-indent>\n";
        }
    }

    public void closeMissionModal()
    {
        missionModal.SetActive(false);
    }

    public void closeItemsModal()
    {
        itemsModal.SetActive(false);
    }

    IEnumerator addTestingMission()
    {
        yield return new WaitForSeconds(10);
        if (GameManager.Instance.Player.GetComponent<Player>().acceptedMissions.Count == 0)
            GameManager.Instance.Player.GetComponent<Player>().AddMission(missions[0]);
    }
}
