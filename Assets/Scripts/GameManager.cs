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
        DialogManager.Hide();

        StartCoroutine(InitialLoad());
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Player p = Player.GetComponent<Player>();
            SaveManager.SavePlayerData(new PlayerData(p));
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerData pd = SaveManager.LoadPlayerData();
            Player.GetComponent<Player>().loadFromPlayerData(pd);
        }
    }

    IEnumerator InitialLoad()
    {
        yield return new WaitForSeconds(3);

        PlayerData pd = SaveManager.LoadPlayerData();
        Player.GetComponent<Player>().loadFromPlayerData(pd);
    }
}
