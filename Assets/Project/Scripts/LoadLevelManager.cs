using UnityEngine;
using System.Collections;

public class LoadLevelManager : MonoBehaviour {

    private static LoadLevelManager _instance;

    public static LoadLevelManager instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = new GameObject().AddComponent<LoadLevelManager>();
                _instance.name = "LoadLevelManager";
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public void openLevel(string name)
    {
        StartCoroutine(loadLevel(name));
    }

    private IEnumerator loadLevel(string name)
    {
       yield return Application.LoadLevelAsync("Loading");
       Application.LoadLevelAsync(name);
    }

}
