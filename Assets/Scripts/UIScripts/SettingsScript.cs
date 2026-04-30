using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    // --------------------------------------------------------------------------- Variables

    #region Public Variables

    // Singleton instance for easy access
    public static SettingsScript Instance;
    void Awake()
    {
        Instance = this;

        VisableSettingsMenu("hide");
        // Input system setup
        input = new InputSys();
    }

    void OnEnable()
    {
        input.UI.Enable();

        input.UI.ToggleMenu.performed += OnToggleMenu;
    }

    void OnDisable()
    {
        input.UI.Disable();

        input.UI.ToggleMenu.performed -= OnToggleMenu;
    }

    private void OnToggleMenu(InputAction.CallbackContext ctx)
    {
        if (PauseScript.isPaused)
        {
            VisableSettingsMenu("hide");
            SaveAllVolume();
            PlayerPrefs.Save();
        }
    }

    public float MasterVolume = 0.2f;
    public float MusicVolume = 0.1f;
    public float SFXVolume = 0.1f;

    public UIDocument SettingsMenuUI;
    public AudioMixer mixer;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    private Button ButtonRAP;
    private Slider SliderVolume;
    private Slider SliderMusic;
    private Slider SliderSFX;

    // For startup check
    int isFirstStart;

    // Input system variables
    private InputSys input;

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- Call

    #region Call

    public void CallSettings()
    {
        VisableSettingsMenu("show");
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    // -------------------------------------------------- UI Logic

    #region UI Logic

    void VisableSettingsMenu(string what)
    {
        if (what == "show")
        {
            SettingsMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else if (what == "hide")
        {
            SettingsMenuUI.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    #endregion

    // -------------------------------------------------- Volume Control

    #region Volume Control

    float LinearToDb(float value)
    {
        //value = Mathf.Clamp01(value);
        ////Debug.Log($"Volume: {value} (dB: {Mathf.Lerp(-80f, 0f, value)})");
        //return Mathf.Lerp(-80f, 0f, value);
        value = Mathf.Clamp(value, 0.0001f, 1f); // avoid log(0)
        float dB = 20f * Mathf.Log10(value);
        return dB;

    }

    void SaveAllVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }

    void LoadAllVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SliderVolume.SetValueWithoutNotify(MasterVolume);
        SliderMusic.SetValueWithoutNotify(MusicVolume);
        SliderSFX.SetValueWithoutNotify(SFXVolume);
        mixer.SetFloat("MasterVolume", LinearToDb(MasterVolume));
        mixer.SetFloat("MusicVolume", LinearToDb(MusicVolume));
        mixer.SetFloat("SFXVolume", LinearToDb(SFXVolume));
    }

    #endregion

    #endregion Functions

    // --------------------------------------------------------------------------- Start and Update

    #region Unity Lifecycle Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ButtonRAP = SettingsMenuUI.rootVisualElement.Q<Button>("ButtonRAP");
        SliderVolume = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderVolume");
        SliderMusic = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderMusic");
        SliderSFX = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderSFX");

        isFirstStart = PlayerPrefs.GetInt("IsFirstStart", 1);
        if (isFirstStart == 1)
        {
            SaveAllVolume();
            PlayerPrefs.SetInt("IsFirstStart", 0);
            Debug.Log("First start");
        }

        LoadAllVolume();

        ButtonRAP.clicked += () =>
        {
            PlayerPrefs.DeleteAll();
            PauseScript.Instance.TogglePause();
            ReloadScene();
        };

        SliderVolume.RegisterValueChangedCallback(evt =>
        {
            MasterVolume = evt.newValue;
            mixer.SetFloat("MasterVolume", LinearToDb(MasterVolume));
        });

        SliderMusic.RegisterValueChangedCallback(evt =>
        {
            MusicVolume = evt.newValue;
            mixer.SetFloat("MusicVolume", LinearToDb(MusicVolume));
        });

        SliderSFX.RegisterValueChangedCallback(evt =>
        {
            SFXVolume = evt.newValue;
            mixer.SetFloat("SFXVolume", LinearToDb(SFXVolume));
        });
    }
    #endregion
}