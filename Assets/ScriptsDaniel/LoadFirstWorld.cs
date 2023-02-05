using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstWorld : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SaveManager.SaveInGameData(
                new PlayerData(GameManager.Instance.Player.GetComponent<Player>()),
                new MissionWrapper(MissionManager.Instance.Missions),
                new InteractionWrapper(GameManager.Instance.interactions)
            );

            SceneManager.LoadScene("FirstWorld", LoadSceneMode.Single);
        }
    }

}
