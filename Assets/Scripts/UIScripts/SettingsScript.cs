using UnityEngine;
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

    public UIDocument SettingsMenuUI;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    // Private variables go here

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

    // -------------------------------------------------- XXX

    #region Cat3

    // Code for Functions Layer 3

    #endregion

    #endregion Functions

    // --------------------------------------------------------------------------- Start and Update

    #region Unity Lifecycle Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisableSettingsMenu("hide");
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    // --------------------------------------------------------------------------- Collision and Trigger Events

    #region Collision and Trigger Events



    #endregion

    // --------------------------------------------------------------------------- End of Script
}
