using Doublsb.Dialog;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickable : MonoBehaviour
{
    public bool picked = false;

    void Awake()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            if (picked == false)
            {
                GameManager.Instance.Player.GetComponent<Player>().inventory.Add(this.transform.gameObject);
                
                DialogData dialogData = new DialogData("/sound:itemCollected/Item recogido: " + this.name + "/click//close/", "Player");
                GameManager.Instance.DialogManager.Show(dialogData);

                this.transform.gameObject.SetActive(false); 
                picked = true;
                Debug.Log("Item: " + this.transform.gameObject.name + " recogido");
            }
        }
    }
}
