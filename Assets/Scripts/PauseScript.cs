using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public static bool isPaused = false;

    public static PauseScript Instance;

    void Awake()
    {
        Instance = this;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
            Pause();
        else
            Resume();
    }

    void Pause()
    {
        Time.timeScale = 0f;
    }

    void Resume()
    {
        Time.timeScale = 1f;
    }
}
