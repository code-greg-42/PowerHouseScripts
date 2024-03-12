using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Attributes")]
    public float attackForce = 500.0f;
    public float attackPower = 34.0f;

    [Header("Cooldown UI Components")]
    public TextMeshProUGUI goldenGunCooldownText;
    public TextMeshProUGUI slowTimeCooldownText;
    public TextMeshProUGUI forcePullCooldownText;

    // slow time cooldown variables
    private bool slowTimeActive;
    private bool slowTimeOnCooldown;
    private float slowTimeCooldownTimer = 10.0f;
    private int slowTimeDuration = 3;
    private float slowTimeCooldownLength = 10.0f;
    private float slowTimeLastUpdatedTime = 0f;

    // golden gun cooldown variables
    private bool goldenGunOnCooldown;
    private float goldenGunCooldownTimer = 15.0f;
    private int goldenGunDuration = 5;
    private float goldenGunCooldownLength = 15.0f;
    private float goldenGunLastUpdatedTime = 0f;

    [HideInInspector]
    public bool goldenGunActive;

    // force pull cooldown variables
    private bool forcePullOnCooldown;
    private float forcePullCooldownTimer = 12.0f;
    private float forcePullCooldownLength = 12.0f;
    private readonly float forcePullCenterDistance = 5.0f;
    private readonly int forcePullHoldTime = 1;
    private readonly float forcePullStrength = 200.0f;
    private float forcePullLastUpdatedTime = 0f;

    // game manager script reference
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            // Main Attack using raycasting
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.AddForce(ray.direction * attackForce);
                    }
                    if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                    {
                        enemy.TakeDamage(attackPower);
                    }
                }
            }

            // Slow Time input
            if (Input.GetKeyDown(KeyCode.F) && !slowTimeOnCooldown && !slowTimeActive)
            {
                slowTimeOnCooldown = true;
                StartCoroutine(SlowTime());
            }

            // track slow time cooldown with timer and boolean
            if (slowTimeOnCooldown)
            {
                slowTimeCooldownTimer -= Time.deltaTime;

                // round the timer to nearest tenth of a second
                float roundedTimer = Mathf.Round(slowTimeCooldownTimer * 10f) / 10f;
                // check if the rounded timer has changed
                if (Mathf.Abs(roundedTimer - slowTimeLastUpdatedTime) >= 0.1f)
                {
                    slowTimeCooldownText.text = "Slow Time: " + roundedTimer.ToString("F1");
                    slowTimeLastUpdatedTime = roundedTimer;
                }

                if (slowTimeCooldownTimer <= 0)
                {
                    slowTimeOnCooldown = false;
                    slowTimeCooldownTimer = slowTimeCooldownLength;
                    slowTimeCooldownText.text = "Slow Time: Ready!";
                }
            }

            // Golden Gun input
            if (Input.GetKeyDown(KeyCode.E) && !goldenGunOnCooldown && !goldenGunActive)
            {
                goldenGunOnCooldown = true;
                StartCoroutine(GoldenGun());
            }

            // track golden gun cooldown with timer and boolean
            if (goldenGunOnCooldown)
            {
                goldenGunCooldownTimer -= Time.deltaTime;

                // round the timer to nearest tenth of a second
                float roundedTimer = Mathf.Round(goldenGunCooldownTimer * 10f) / 10f;
                // check if the rounded timer has changed
                if (Mathf.Abs(roundedTimer - goldenGunLastUpdatedTime) >= 0.1f)
                {
                    goldenGunCooldownText.text = "Golden Gun: " + roundedTimer.ToString("F1");
                    goldenGunLastUpdatedTime = roundedTimer;
                }

                if (goldenGunCooldownTimer <= 0)
                {
                    goldenGunOnCooldown = false;
                    goldenGunCooldownTimer = goldenGunCooldownLength;
                    goldenGunCooldownText.text = "Golden Gun: Ready!";
                }
            }

            // Force Pull input
            if (Input.GetKeyDown(KeyCode.C) && !forcePullOnCooldown)
            {
                forcePullOnCooldown = true;
                ForcePull();
            }

            // track force pull cooldown with timer and boolean
            if (forcePullOnCooldown)
            {
                forcePullCooldownTimer -= Time.deltaTime;

                // round the timer to nearest tenth of a second
                float roundedTimer = Mathf.Round(forcePullCooldownTimer * 10f) / 10f;
                // check if the rounded timer has changed
                if (Mathf.Abs(roundedTimer - forcePullLastUpdatedTime) >= 0.1f)
                {
                    forcePullCooldownText.text = "Force Pull: " + roundedTimer.ToString("F1");
                    forcePullLastUpdatedTime = roundedTimer;
                }

                if (forcePullCooldownTimer <= 0)
                {
                    forcePullOnCooldown = false;
                    forcePullCooldownTimer = forcePullCooldownLength;
                    forcePullCooldownText.text = "Force Pull: Ready!";
                }
            }
        }
    }

    IEnumerator SlowTime()
    {
        slowTimeActive = true;
        // save current attack force
        float originalAttackForce = attackForce;
        // lower attack force to give consistency to slow time effect
        attackForce *= 0.3f;

        // find all enemies
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                // stops random movement method from enemy script
                enemyScript.StopMoving();
            }
            if (enemy.TryGetComponent<Rigidbody>(out var rb))
            {
                // slows speed
                rb.velocity *= 0.3f;
                rb.angularVelocity *= 0.3f;
            }
        }
        yield return new WaitForSeconds(slowTimeDuration);

        foreach(Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                // restarts random movement method from enemy script
                enemyScript.StartMoving(0);
            }
        }
        // reset attack force to the original amount
        attackForce = originalAttackForce;

        slowTimeActive = false;
    }

    IEnumerator GoldenGun()
    {
        goldenGunActive = true;

        // save current attack power
        float originalAttackPower = attackPower;
        // max out attack power
        attackPower = 100.0f;

        yield return new WaitForSeconds(goldenGunDuration);

        goldenGunActive = false;
        // return attack power to normal amount
        attackPower = originalAttackPower;
    }

    IEnumerator MoveToCenter(Rigidbody rb, Enemy enemy)
    {
        if (enemy != null)
        {
            // stop repeating sporadic movement
            enemy.StopMoving();
        }

        if (rb != null)
        {
            // initially stop all movement to halt current momentum
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // move towards center until it gets close
        while (rb != null && Vector3.Distance(Vector3.zero, rb.position) > forcePullCenterDistance)
        {
            // aim at center and add force according to force strength
            // .normalized is not used to get all objects to center at similar time, so the further the object the faster it moves
            Vector3 originDirection = Vector3.zero - rb.position;
            rb.AddForce(originDirection * forcePullStrength, ForceMode.Force);
            // using WaitForFixedUpdate for consistency in WebGL
            yield return new WaitForFixedUpdate();
        }

        if (rb != null)
        {
            // stop object when it reaches center spot
            rb.constraints = RigidbodyConstraints.FreezeAll;
            // wait for allotted amount of time
            yield return new WaitForSeconds(forcePullHoldTime);
            
        }

        if (rb != null && enemy != null)
        {
            // remove rigidbody constraints to allow for movement
            rb.constraints = RigidbodyConstraints.None;
            // restart sporadic movement
            enemy.StartMoving(0);
        }
    }

    void ForcePull()
    {
        // get all enemies in scene and move each to center
        foreach(Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.TryGetComponent(out Rigidbody enemyRb))
            {
                StartCoroutine(MoveToCenter(enemyRb, enemy));
            }
        }
    }
}
