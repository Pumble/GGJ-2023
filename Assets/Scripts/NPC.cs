using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

/**
 * Referencias:
 * Documentacion DDialog: https://github.com/DoublSB/UnityDialogAsset
 */
public class NPC : MonoBehaviour
{
    #region Variables
    [SerializeField] private string name;
    [SerializeField] private string code;
    private Dialogue defaultDialogue;

    public DialogManager dialogManager;
    #endregion

    #region Propiedades
    public string Name { get => name; set => name = value; }
    public string Code { get => code; set => code = value; }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("El jugador quiere interactuar con: " + this.Code);
            GameManager.Instance.Player.GetComponent<Player>().Interact(this.Code, this.gameObject);
        }
    }

    public void startDefaultDialogue()
    {
        // Aqui iniciamos el dialogo por default
        DialogData dialogData = new DialogData(this.defaultDialogue.Text, this.Code);
        dialogManager.Show(dialogData);
    }
}