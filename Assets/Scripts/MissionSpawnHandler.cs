using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSpawnHandler : MonoBehaviour
{
    private bool missionAccepted = false;

    #region Triggers

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            if (missionAccepted == false)
            {
                Mission m = this.transform.gameObject.GetComponent<Mission>();
                GameManager.Instance.Player.GetComponent<Player>().AddMission(m);
                missionAccepted = true;
            }
        }
    }

    #endregion
}
