using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void loadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
