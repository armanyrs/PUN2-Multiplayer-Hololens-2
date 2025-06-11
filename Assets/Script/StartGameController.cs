using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameController : MonoBehaviour
{
    public void OnPlayPressed()
    {
        Debug.Log("TOMBO DITEKAN!"); // Pastikan ada ini
        SceneManager.LoadScene("Game_scene");
    }
}