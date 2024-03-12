using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Attributes")]
    public float health = 100f;
    public int points;

    [Header("Movement Attributes")]
    public float movementSpeed = 10.0f;
    public float movementInterval = 1.5f;
    
    private readonly float maxHealth = 100f;
    private readonly float healDelay = 3.0f;
    private readonly float healAmount = 34f;
    private readonly float hurtThreshold = 66.6f;
    private readonly float criticalThreshold = 33.3f;
    private readonly float outOfBoundsRange = 16.0f;

    private new Renderer renderer;
    private Rigidbody enemyRb;

    private Coroutine healCoroutine;
    private PlayerAttack playerAttack;
    private GameManager gameManager;
    private AudioSource playerAudio;

    [Header("Effects and Sounds")]
    public ParticleSystem explosionParticle;
    public ParticleSystem goldenGunParticle;
    public AudioClip deathAudio;
    
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        enemyRb = GetComponent<Rigidbody>();
        playerAttack = GameObject.Find("PlayerAttack").GetComponent<PlayerAttack>();
        playerAudio = GameObject.Find("PlayerAttack").GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // initial color update --- should always be green
        UpdateColor();
        // start movement method for sporadic movement
        StartMoving(0);
    }

    // Update is called once per frame
    void Update()
    {
        // destroy enemy if it becomes out of bounds
        if (transform.position.x < -outOfBoundsRange || transform.position.x > outOfBoundsRange || transform.position.y < -2
            || transform.position.y > outOfBoundsRange || transform.position.z < -outOfBoundsRange ||
            transform.position.z > outOfBoundsRange)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        // subtract damage amount from health total
        health -= damage;
        UpdateColor();

        // restart the timer on healing since damage has occurred
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
        }
        healCoroutine = StartCoroutine(Heal());

        // destroy object if health drops to 0
        if (health <= 0)
        {
            if (playerAttack.goldenGunActive)
            {
                Instantiate(goldenGunParticle, transform.position, Quaternion.identity);
            } else
            {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
            }
            playerAudio.PlayOneShot(deathAudio, 1.0f);
            gameManager.AddPoints(points);
            Destroy(gameObject);
        }
    }

    IEnumerator Heal()
    {
        while (health < maxHealth)
        {
            // wait after taking damage to heal up
            yield return new WaitForSeconds(healDelay);

            // restore health by allotted amount
            health += healAmount;

            // ensure health does not exceed max health
            health = Mathf.Min(health, maxHealth);

            // update the object color for new health amount
            UpdateColor();
        }
    }

    void UpdateColor()
    {
        // change color of object based on health
        if (health <= criticalThreshold)
        {
            renderer.material.color = Color.red;
        }
        else if (health <= hurtThreshold)
        {
            renderer.material.color = Color.yellow;
        }
        else
        {
            renderer.material.color = Color.green;
        }
    }

    void MoveEnemy()
    {
        // randomize direction of force
        Quaternion randomRotation = Random.rotation;
        Vector3 randomDirection = randomRotation * Vector3.forward;
        // add impulse force for sporadic movement
        enemyRb.AddForce(movementSpeed * randomDirection, ForceMode.Impulse);
    }

    public void StartMoving(int initialDelay)
    {
        InvokeRepeating(nameof(MoveEnemy), initialDelay, movementInterval);
    }

    public void StopMoving()
    {
        CancelInvoke(nameof(MoveEnemy));
    }

    public void StopHeal()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }
}
