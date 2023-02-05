using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSecondWorld : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D other) {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene("SecondWorld", LoadSceneMode.Single);
        }
    }
}
