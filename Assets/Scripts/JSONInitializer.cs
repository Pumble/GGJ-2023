using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class JSONInitializer : MonoBehaviour
{
    private void Awake()
    {
        List<string> paths = new List<string>();
        paths.Add("interactions");
        paths.Add("missions");
        paths.Add("player");

        foreach (string p in paths)
        {
            string newPath = Application.persistentDataPath + "/" + p + ".json";
            if (!File.Exists(newPath))
            {
                // TextAsset targetFile = Resources.Load<TextAsset>("interactions");
                string origin = "Assets/JSON/" + p + ".json";
                File.Copy(origin, newPath);
            }
        }
    }
}
