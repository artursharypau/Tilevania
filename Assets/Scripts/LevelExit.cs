using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private static readonly WaitForSecondsRealtime LoadingDelay = new(0.5f);

    public static UnityEvent LevelExited { get; } = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(LoadNextLevel());
    }

    private static IEnumerator LoadNextLevel()
    {
        LevelExited.Invoke();

        yield return LoadingDelay;

        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            print("The current level is the last one for now");
            nextLevelIndex = 0;
        }

        SceneManager.LoadScene(nextLevelIndex);
    }
}
