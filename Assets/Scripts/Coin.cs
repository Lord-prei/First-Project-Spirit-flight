using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    // --------------------------------------------------------------------------- Variables

    #region Public Variables

    int whiteCoinValue = 1;
    int yellowCoinValue = 3;
    int blueCoinValue = 5;

    // Coin span chances (out of 100)
    int whiteCoinChance = 75;
    int yellowCoinChance = 20;
    int blueCoinChance = 5;

    public GameObject coinSFXPrefab;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    private int money = 0;
    private int moneyValue = 0;
    private bool isCollected = false;

    Renderer rend;
    Rigidbody2D rb;

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- RNG

    #region RNG

    bool CheckRNG()
    {
        int totalPercentage = whiteCoinChance + yellowCoinChance + blueCoinChance;
        if (totalPercentage == 100)
        {
            return true;
        }
        //Debug.Log($"ERROR MAX PERCENTAGE NOT 100%, ITS: {totalPercentage}");
        return false;
    }
    void CoinValueRNG()
    {
        if (CheckRNG())
        {
            int randomValue = Random.Range(0, 100);

            if (randomValue < whiteCoinChance)
            {
                // White Coin
                moneyValue = whiteCoinValue;
                rend.material.color = Color.white;
            }
            else if (randomValue < whiteCoinChance + yellowCoinChance)
            {
                // Yellow Coin
                moneyValue = yellowCoinValue;
                rend.material.color = Color.yellow;
            }
            else if (randomValue < whiteCoinChance + yellowCoinChance + blueCoinChance)
            {
                // Blue Coin
                moneyValue = blueCoinValue;
                rend.material.color = Color.blue;
            }

            //Debug.Log($"Coin Value: {moneyValue} (Random Value: {randomValue})");
        }
    }

    #endregion

    // -------------------------------------------------- XXX

    #region Cat2

    // Code for Functions Layer 2

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
        rend = GetComponent<Renderer>();
        money = PlayerPrefs.GetInt("Money", 0);
        CoinValueRNG();
    }

    #endregion

    // --------------------------------------------------------------------------- Collision and Trigger Events

    #region Collision and Trigger Events

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCollected) return; // Prevent multiple collections
            isCollected = true;

            money = PlayerPrefs.GetInt("Money", 0); // Get the updated money value

            //Debug.Log($"Coin collected!, Total Money: {money}");

            rend.enabled = false;
            StartCoroutine(CoinUpdater(moneyValue));
        }
    }

    System.Collections.IEnumerator CoinUpdater(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject coinPickupSFX = Instantiate(coinSFXPrefab, coinSFXPrefab.transform.position, Quaternion.identity);
            PlayerController.Instance.UpdateMoney(1); // Increment money by 1
            yield return new WaitForSeconds(0.05f);
            Destroy(coinPickupSFX, 1f);
        }
        
        Destroy(gameObject);
    }

    #endregion

    // --------------------------------------------------------------------------- End of Script
}