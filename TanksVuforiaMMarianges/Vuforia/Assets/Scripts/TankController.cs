using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    public GameObject stickL; // Referencia al GameObject del joystick izquierdo
    private StickController joystickInput; // Referencia al script del StickController

    public Transform cannon; // Referencia al cañón
    public Transform shootingPoint;
    public GameObject projectilePrefab; // Prefab del proyectil
    public float yawSpeed = 5f; // Velocidad de giro en yaw
    public float pitchSpeed = 5f; // Velocidad de giro en pitch
    public float maxLaunchForce = 20f; // Fuerza máxima del disparo
    public float chargeRate = 5f; // Tasa de carga de la fuerza del disparo
    public Slider healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    private float yawInput;
    private float pitchInput;
    private float currentLaunchForce;
    private bool aiming;
    public LineRenderer aimLine;
    public bool justFired;

    public AudioClip impactClip;
    public AudioClip fireClip; // Nuevo AudioClip para el sonido de disparo
    private AudioSource audioSource;

    private GameManager gameManager;

    void Start()
    {
        aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.enabled = false;
        justFired = false;
        currentHealth = maxHealth;

        gameManager = FindObjectOfType<GameManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (impactClip == null)
        {
            Debug.LogWarning("Impact clip is not assigned.");
        }
        if (fireClip == null)
        {
            Debug.LogWarning("Fire clip is not assigned.");
        }

        // Obtener la referencia al script de entrada del joystick
        if (stickL != null)
        {
            joystickInput = stickL.GetComponent<StickController>();
            if (joystickInput != null)
            {
                joystickInput.StickChanged += OnStickChanged;
            }
        }
    }

    void Update()
    {
        if (!justFired)
        {
            MoveCannon();
            UpdateLaunchForce();
            UpdateAimLine();

            // Disparar cuando se presiona la barra espaciadora
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCharging();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Fire();
            }
        }
    }

    void OnStickChanged(object sender, StickEventArgs e)
    {
        yawInput = e.Position.x;
        pitchInput = e.Position.y;
    }

    void MoveCannon()
    {
        float yawDelta = yawInput * yawSpeed * Time.deltaTime;
        float pitchDelta = pitchInput * pitchSpeed * Time.deltaTime;

        cannon.Rotate(Vector3.up, yawDelta, Space.Self);
        cannon.Rotate(Vector3.right, -pitchDelta, Space.Self);
    }

    void UpdateLaunchForce()
    {
        if (aiming)
        {
            currentLaunchForce += chargeRate * Time.deltaTime;
            currentLaunchForce = Mathf.Clamp(currentLaunchForce, 0f, maxLaunchForce);
        }
    }

    void UpdateAimLine()
    {
        Vector3 startPoint = cannon.position;
        Vector3 endPoint = startPoint + cannon.forward * currentLaunchForce * 0.5f;
        aimLine.SetPosition(0, startPoint);
        aimLine.SetPosition(1, endPoint);
    }

    void StartCharging()
    {
        aiming = true;
        currentLaunchForce = 0f;
        aimLine.enabled = true;
    }

    void Fire()
    {
        aiming = false;
        aimLine.enabled = false;

        Quaternion projectileRotation = Quaternion.Euler(cannon.rotation.eulerAngles.x - -90f, cannon.rotation.eulerAngles.y, cannon.rotation.eulerAngles.z);
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, projectileRotation);
        Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = currentLaunchForce * cannon.forward;
        justFired = true;

        // Reproducir el sonido de disparo
        if (fireClip != null)
        {
            audioSource.PlayOneShot(fireClip);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthFill();

        // Reproducir el sonido de impacto
        if (impactClip != null)
        {
            audioSource.PlayOneShot(impactClip);
        }

        // Notificar al GameManager si la vida llega a 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameManager.PlayerDied(this);
        }
    }

    private void UpdateHealthFill()
    {
        healthBar.value = currentHealth;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    void OnDestroy()
    {
        if (joystickInput != null)
        {
            joystickInput.StickChanged -= OnStickChanged;
        }
    }
}
