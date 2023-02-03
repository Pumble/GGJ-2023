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
    [SerializeField] private string code;
    [SerializeField] private string defaultDialogue;
    #endregion

    #region Propiedades
    public string Code { get => code; set => code = value; }
    public string DefaultDialogue { get => defaultDialogue; set => defaultDialogue = value; }
    #endregion

    #region Triggers

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.Player.GetComponent<Player>().Interact(this.Code, this.transform.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.Player.GetComponent<Player>().Interacting = false;
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
            Debug.Log("Dialogo finalizado");
            GameManager.Instance.Player.GetComponent<Player>().Interacting = false;
        };
    }
}