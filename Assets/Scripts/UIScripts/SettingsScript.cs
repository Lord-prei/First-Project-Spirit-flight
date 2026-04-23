using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsScript : MonoBehaviour
{
    // --------------------------------------------------------------------------- Variables

    #region Public Variables

    // Singleton instance for easy access
    public static SettingsScript Instance;
    void Awake()
    {
        Instance = this;
    }

    public float MasterVolume = 0.8f;
    public float MusicVolume = 0.4f;
    public float SFXVolume = 0.2f;

    public UIDocument SettingsMenuUI;
    public AudioMixer mixer;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    private Button ButtonRAP;
    private Slider SliderVolume;
    private Slider SliderMusic;
    private Slider SliderSFX;

    private int startup = 0;
    private bool once = false;

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- Call

    #region Call

    public void CallSettings()
    {
        once = false;
        LoadAllSliders();
        VisableSettingsMenu("show");
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
        float linear = Mathf.Pow(value, 2.0f);
        linear = Mathf.Clamp(linear, 0.0001f, 1f);
        float db = 20f * Mathf.Log10(linear);
        Debug.Log($"Linear: {value}, dB: {db}");
        return db;
    }

    void SetAllVolume()
    {
        mixer.SetFloat("MasterVolume", LinearToDb(MasterVolume));
        mixer.SetFloat("MusicVolume", LinearToDb(MusicVolume));
        mixer.SetFloat("SFXVolume", LinearToDb(SFXVolume));

        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
    }

    void LoadAllVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", MasterVolume);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);

        mixer.SetFloat("MasterVolume", LinearToDb(MasterVolume));
        mixer.SetFloat("MusicVolume", LinearToDb(MusicVolume));
        mixer.SetFloat("SFXVolume", LinearToDb(SFXVolume));
    }

    void LoadAllSliders()
    {         
        SliderVolume.SetValueWithoutNotify(MasterVolume);
        SliderMusic.SetValueWithoutNotify(MusicVolume);
        SliderSFX.SetValueWithoutNotify(SFXVolume);
    }

    #endregion

    #endregion Functions

    // --------------------------------------------------------------------------- Start and Update

    #region Unity Lifecycle Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisableSettingsMenu("hide");

        ButtonRAP = SettingsMenuUI.rootVisualElement.Q<Button>("ButtonRAP");
        SliderVolume = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderVolume");
        SliderMusic = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderMusic");
        SliderSFX = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderSFX");

        startup = PlayerPrefs.GetInt("Startup", 0);

        if (startup == 0)
        {
            Debug.Log("First startup detected, loading default volume settings.");
            LoadAllSliders();
            PlayerPrefs.SetInt("Startup", 1);

            Debug.Log($"Default volumes set: Master={MasterVolume}, Music={MusicVolume}, SFX={SFXVolume}");
        }
        else
        {
            LoadAllVolume();
        }

            ButtonRAP.clicked += () =>
            {
                Debug.Log("RAP Button Pressed");
            };

        SliderVolume.RegisterValueChangedCallback(evt =>
        {
            MasterVolume = evt.newValue;
            SetAllVolume();
        });

        SliderMusic.RegisterValueChangedCallback(evt =>
        {
            MusicVolume = evt.newValue;
            SetAllVolume();
        });

        SliderSFX.RegisterValueChangedCallback(evt =>
        {
            SFXVolume = evt.newValue;
            SetAllVolume();
        });
    }


    // Update is called once per frame


    void Update()
    {
        if (!PauseScript.isPaused)
        {
            if (!once)
            {
                once = true;
                VisableSettingsMenu("hide");
                PlayerPrefs.Save();
                LoadAllVolume();

                Debug.Log($"Settings saved: Master={MasterVolume}, Music={MusicVolume}, SFX={SFXVolume}");
            }
            
        }
    }

    #endregion

    // --------------------------------------------------------------------------- Collision and Trigger Events

    #region Collision and Trigger Events



    #endregion

    // --------------------------------------------------------------------------- End of Script
}
