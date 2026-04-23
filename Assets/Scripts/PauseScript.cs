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

    public UIDocument PauseMenuUI;
    public UIDocument SettingsMenuUI;
    public UIDocument ShopMenuUI;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    private Button ButtonContinue;
    private Button ButtonSettings;
    private Button ButtonShop;
    private Button ButtonRetard;

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
            PauseMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else if (what == "hide")
        {
            PauseMenuUI.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    #endregion

    // -------------------------------------------------- Scene Management

    #region Scene Management

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #endregion Functions

    // --------------------------------------------------------------------------- Start and Update

    #region Unity Lifecycle Methods

    void Start()
    {
        VisablePauseMenu("hide");
        ButtonContinue  = PauseMenuUI.rootVisualElement.Q<Button>("ButtonContinue");
        ButtonSettings  = PauseMenuUI.rootVisualElement.Q<Button>("ButtonSettings");
        ButtonShop      = PauseMenuUI.rootVisualElement.Q<Button>("ButtonShop");
        ButtonRetard    = PauseMenuUI.rootVisualElement.Q<Button>("ButtonRestart");

        ButtonContinue.clicked += () =>
        {
            TogglePause();
        };

        ButtonSettings.clicked += () =>
        {
            Debug.Log("Settings Button Clicked");
        };

        ButtonShop.clicked += () =>
        {
            Debug.Log("Shop Button Clicked");
        };

        ButtonRetard.clicked += () =>
        {
            ReloadScene();
            Time.timeScale = 1f;
            TogglePause();
        };
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
