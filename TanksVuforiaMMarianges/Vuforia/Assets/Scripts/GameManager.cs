using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    private TankController player1Controller;
    private TankController player2Controller;
    private GameObject activePlayer;

    public GameObject player1TurnSprite;
    public GameObject player2TurnSprite;

    public GameObject player1WinSprite;
    public GameObject player2WinSprite;

    public GameObject restartButton; // Referencia al botón de reinicio

    public AudioClip winSound; // Clip de sonido de victoria
    public AudioClip backgroundMusic; // Clip de música de fondo
    private AudioSource audioSource; // AudioSource para reproducir sonidos

    // Start is called before the first frame update
    void Start()
    {
        player1Controller = player1.GetComponent<TankController>();
        player2Controller = player2.GetComponent<TankController>();

        // Inicializar el primer jugador activo
        activePlayer = player1;
        player1Controller.enabled = true;
        player2Controller.enabled = false;

        // Mostrar el sprite del turno del jugador 1
        player1TurnSprite.SetActive(true);
        player2TurnSprite.SetActive(false);

        // Ocultar los sprites de victoria al inicio
        player1WinSprite.SetActive(false);
        player2WinSprite.SetActive(false);

        // Ocultar el botón de reinicio al inicio
        restartButton.SetActive(false);

        // Configurar el AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        // Reproducir la música de fondo
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // Hacer que la música se repita en bucle
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTurn();
    }

    void CheckTurn()
    {
        if (player1Controller.justFired)
        {
            SwitchTurn(player2);
            player1Controller.justFired = false; // Reinicia el estado después de cambiar de turno
        }
        else if (player2Controller.justFired)
        {
            SwitchTurn(player1);
            player2Controller.justFired = false; // Reinicia el estado después de cambiar de turno
        }
    }

    public void PlayerDied(TankController deadPlayer)
    {
        // Desactivar controles de ambos jugadores
        player1Controller.enabled = false;
        player2Controller.enabled = false;

        // Ocultar los sprites de turno
        player1TurnSprite.SetActive(false);
        player2TurnSprite.SetActive(false);

        // Mostrar el sprite de victoria correspondiente
        if (deadPlayer == player1Controller)
        {
            player2WinSprite.SetActive(true);
        }
        else if (deadPlayer == player2Controller)
        {
            player1WinSprite.SetActive(true);
        }

        // Detener la música de fondo
        audioSource.Stop();

        // Reproducir el sonido de victoria
        audioSource.PlayOneShot(winSound);

        // Mostrar el botón de reinicio
        restartButton.SetActive(true);
    }

    void SwitchTurn(GameObject nextPlayer)
    {
        // Desactiva el jugador actual y activa el siguiente
        activePlayer.GetComponent<TankController>().enabled = false;
        activePlayer = nextPlayer;
        activePlayer.GetComponent<TankController>().enabled = true;

        // Actualizar la visibilidad del sprite del turno
        if (activePlayer == player1)
        {
            player1TurnSprite.SetActive(true);
            player2TurnSprite.SetActive(false);
        }
        else if (activePlayer == player2)
        {
            player1TurnSprite.SetActive(false);
            player2TurnSprite.SetActive(true);
        }
    }

    public void RestartGame()
    {
        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
