using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCredits : MonoBehaviour
{
    private void OnTriggerStay2D(Collision2D other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene("Credits", LoadSceneMode.Single);

        }
    }
}
