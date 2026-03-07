using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(++index);
    }
}
