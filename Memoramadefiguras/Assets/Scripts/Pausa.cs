using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Pausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    public void PausaJuego()
    {
        Debug.Log("Pausando el juego...");
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f; // Aseg�rate de que el tiempo est� activo
        botonPausa.SetActive(true); // Aseg�rate de mostrar el bot�n de pausa
        menuPausa.SetActive(false); // Aseg�rate de ocultar el men� de pausa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena
    }

    public void Cerrar()
    {
        Debug.Log("cerrando jueg");
        Application.Quit();
    }
}
