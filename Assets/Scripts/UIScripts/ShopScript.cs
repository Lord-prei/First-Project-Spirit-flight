using UnityEngine;
using UnityEngine.UIElements;

public class ShopScript : MonoBehaviour
{
    // --------------------------------------------------------------------------- Variables

    #region Public Variables

    // Singleton instance for easy access
    public static ShopScript Instance;
    void Awake()
    {
        Instance = this;
    }

    public UIDocument ShopMenuUI;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    // Private variables go here

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- Call

    #region Call

    public void CallShop()
    {
        VisableShopMenu("show");
    }

    #endregion

    // -------------------------------------------------- UI Logic

    #region UI Logic

    void VisableShopMenu(string what)
    {
        if (what == "show")
        {
            ShopMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else if (what == "hide")
        {
            ShopMenuUI.rootVisualElement.style.display = DisplayStyle.None;
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
        VisableShopMenu("hide");
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
