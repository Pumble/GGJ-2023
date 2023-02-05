using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Referencias:
 * Como hacer un singleton: https://forum.unity.com/threads/help-how-do-you-set-up-a-gamemanager.131170/#post-885626
 */
public class GameManager : MonoBehaviour
{
    #region Datos

    [SerializeField] private DialogManager dialogManager;
    public GameObject Player;
    public GameObject missionModal;

    public List<Interaction> interactions;

    #endregion

    #region Propiedades

    public static GameManager Instance { get; private set; }
    public DialogManager DialogManager { get => dialogManager; set => dialogManager = value; }

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
        DialogData dialogData = new DialogData("/close/");
        GameManager.Instance.DialogManager.Show(dialogData);

        LoadInteractions();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Player p = Player.GetComponent<Player>();
            SaveManager.SavePlayerData(new PlayerData(p));
            SaveManager.SaveInteractionsData(new InteractionWrapper(interactions));
            // SaveManager.SaveMissionsData();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerData pd = SaveManager.LoadPlayerData();
            Player.GetComponent<Player>().loadFromPlayerData(pd);
            LoadInteractions();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            missionModal.SetActive(!missionModal.activeSelf);
        }
    }

    IEnumerator InitialLoad()
    {
        yield return new WaitForSeconds(3);

        PlayerData pd = SaveManager.LoadPlayerData();
        Player.GetComponent<Player>().loadFromPlayerData(pd);
    }

    private void LoadInteractions()
    {
        // Cargar las interacciones
        interactions = SaveManager.LoadInteractionsData();

        // Distribuir las interacciones
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in npcs)
        {
            NPC npcScript = npc.GetComponent<NPC>();
            npcScript.interactions = new List<Interaction>();
            foreach (Interaction interaction in interactions)
            {
                if (interaction.character == npc.name)
                {
                    npcScript.interactions.Add(interaction);
                }
            }
        }
    }
}
