using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHelp : MonoBehaviour
{
    // REFERENCIA
    public void OnBackSceneHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay", LoadSceneMode.Single);
    }
    public void OnBackSceneFirstWorld()
    {
        SceneManager.LoadScene("FirstWorld", LoadSceneMode.Single);
    }

}
