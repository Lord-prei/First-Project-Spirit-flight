using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    // --------------------------------------------------------------------------- Variables

    #region Public Variables

    // Singleton instance for easy access
    public static PauseScript Instance;
    void Awake()
    {
        Instance = this;
    }

    public static bool isPaused = false;

    public UIDocument PausMenuUI;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    // Private variables go here

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- Pause / Resume Logic

    #region Pause / Resume Logic

    void Pause()
    {
        Time.timeScale = 0f;
    }

    void Resume()
    {
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
            Pause();
        else
            Resume();
    }

    #endregion

    // -------------------------------------------------- UI Logic

    #region UI Logic
    void VisablePauseMenu(string what)
    {
        if (what == "show")
        {
            PausMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else if (what == "hide")
        {
            PausMenuUI.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    // Code for Functions Layer 2

    #endregion

    // -------------------------------------------------- XXX

    #region Cat3

    // Code for Functions Layer 3

    #endregion

    #endregion Functions

    // --------------------------------------------------------------------------- Start and Update

    #region Unity Lifecycle Methods

    void Start()
    {
        VisablePauseMenu("hide");
    }

    void Update()
    {
        if (isPaused)
        {
            VisablePauseMenu("show");
        }
        else
        {
            VisablePauseMenu("hide");
        }
    }

    #endregion

    // --------------------------------------------------------------------------- Collision and Trigger Events

    #region Collision and Trigger Events



    #endregion

    // --------------------------------------------------------------------------- End of Script
}
