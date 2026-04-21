using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{

    public AudioSource CoinSFX;

    private int money = 0;
    private bool isCollected = false;

    Rigidbody2D rb;

    // functions

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        money = PlayerPrefs.GetInt("Money", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCollected) return; // Prevent multiple collections
            isCollected = true;


            AudioSource.PlayClipAtPoint(CoinSFX.clip, transform.position);

            PlayerController.Instance.UpdateMoney(1); // Increment money by 1

            money = PlayerPrefs.GetInt("Money", 0); // Get the updated money value

            Debug.Log($"Coin collected!, Total Money: {money}");

            Destroy(gameObject);
        }
    }
}
