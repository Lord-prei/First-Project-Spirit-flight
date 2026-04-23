using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;

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

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- Call

    #region Call

    public void CallSettings()
    {
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
        return 20f * Mathf.Log10(linear);
    }

    void SetAllVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0);

        mixer.SetFloat("MasterVolume", LinearToDb(MasterVolume));
        mixer.SetFloat("MusicVolume", LinearToDb(MusicVolume));
        mixer.SetFloat("SFXVolume", LinearToDb(SFXVolume));
    }

    void LoadAllVolume()
    {         
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
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

        ButtonRAP       = SettingsMenuUI.rootVisualElement.Q<Button>("ButtonRAP");
        SliderVolume    = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderVolume");
        SliderMusic     = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderMusic");
        SliderSFX       = SettingsMenuUI.rootVisualElement.Q<Slider>("SliderSFX");

        LoadAllVolume();

        ButtonRAP.clicked += () =>
        {
            Debug.Log("RAP Button Pressed");
        };

        SliderVolume.RegisterValueChangedCallback(evt =>
        {
            MasterVolume = evt.newValue;
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            SetAllVolume();
        });

        SliderMusic.RegisterValueChangedCallback(evt =>
        {
            MusicVolume = evt.newValue;
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
            SetAllVolume();
        });

        SliderSFX.RegisterValueChangedCallback(evt =>
        {
            SFXVolume = evt.newValue;
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
            SetAllVolume();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseScript.isPaused)
        {
            VisableSettingsMenu("hide");
            PlayerPrefs.Save();
        }
    }

    #endregion

    // --------------------------------------------------------------------------- Collision and Trigger Events

    #region Collision and Trigger Events



    #endregion

    // --------------------------------------------------------------------------- End of Script
}
