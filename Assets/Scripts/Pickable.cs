using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickable : MonoBehaviour
{
    private bool picked = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            if (picked == false)
            {
                GameManager.Instance.Player.GetComponent<Player>().Inventory.Add(this.transform.gameObject);
                picked = true;
            }
        }
    }
}
