using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstWorld : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene("FirstWorld", LoadSceneMode.Single);

        }
    }

}
