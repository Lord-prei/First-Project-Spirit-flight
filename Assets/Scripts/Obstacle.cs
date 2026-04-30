using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    // --------------------------------------------------------------------------- Variables

    #region Public Variables

    public float minSize = 0.5f;
    public float maxSize = 2f;

    public float minSpeed = 50f;
    public float maxSpeed = 250f;

    public float baseMass = 1f;

    public float maxSpinSpeed = 10f;

    public float particleScaleFactor = 1f;
    public float destructionEffectScaleFactor = 1f;
    public float collisionVelocityThreshold = 8f;

    public GameObject collisionEffectPrefab;
    public GameObject destructionEffectPrefab;

    #endregion

    // --------------------------------------------------------------------------- Private Variables

    #region Private Variables

    private float weight;

    Rigidbody2D rb;

    #endregion

    // --------------------------------------------------------------------------- Functions

    #region Functions

    // -------------------------------------------------- XXX

    #region Cat1

    // Code for Functions Layer 1

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
        rb = GetComponent<Rigidbody2D>();

        // Randomly scale the obstacle size
        float xScale = Random.Range(minSize, maxSize);
        float yScale = xScale;
        float zScale = 1;
        transform.localScale = new Vector3(xScale, yScale, zScale);

        // Calculate mass based on size and base mass
        float mass = baseMass * xScale * yScale;
        rb.mass = mass;
        weight = mass;

        // Randomly apply torque to the obstacle
        float torque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(torque);

        // Randomly apply force to the obstacle in a random direction
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 direction = Random.insideUnitCircle;
        rb.AddForce(direction * speed);


        //Debug.Log($"obstacle: {gameObject.name} with \nsize: \t\t{xScale} \nmass: \t{mass} \nspeed: \t{speed} \ntorque: \t{torque}");
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    // --------------------------------------------------------------------------- Collision and Trigger Events

    #region Collision and Trigger Events

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject collisonEffect = Instantiate(collisionEffectPrefab, contactPoint, Quaternion.identity);

        // Scale the collision effect based on the velocity of the obstacle at the time of collision
        float velocityMagnitude = collision.relativeVelocity.magnitude;
        collisonEffect.transform.localScale = Vector3.one * Mathf.Sqrt(velocityMagnitude) * particleScaleFactor;

        // Destroy the obstacle if the velocity magnitude exceeds the threshold and create a destruction effect
        if (velocityMagnitude > collisionVelocityThreshold)
        {
            // Destroy the Obstacle
            Destroy(gameObject);

            // Instantiate the destruction effect at the contact point and scale it based on the weight of the obstacle
            GameObject destructionEffect = Instantiate(destructionEffectPrefab, contactPoint, Quaternion.identity);
            float destructionEffectScale = weight * destructionEffectScaleFactor;
            destructionEffect.transform.localScale = Vector3.one * destructionEffectScale;

            //Debug.Log($"Obstacle destroyed at {contactPoint} with velocity magnitude: {velocityMagnitude}");
            Destroy(destructionEffect, 7f);
        }

        //Debug.Log($"Collision detected at {contactPoint} with velocity magnitude: {velocityMagnitude}");

        // Destroy the collision effect after a short delay to prevent cluttering the scene
        Destroy(collisonEffect, 0.5f);
    }

    #endregion

    // --------------------------------------------------------------------------- End of Script
}
