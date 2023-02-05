using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class MissionSpawnHandler : MonoBehaviour
{
    public bool missionAccepted = false;

    void Awake()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    #region Triggers

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            if (missionAccepted == false)
            {
                GameObject parent = this.transform.parent.gameObject;
                Mission m = parent.GetComponent<Mission>();
                GameManager.Instance.Player.GetComponent<Player>().AddMission(m);
                missionAccepted = true;
                parent.SetActive(false);
                Debug.Log("Mision: " + m.missionName + " aceptada");
            }
        }
    }

    #endregion
}
