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
        Debug.Log("Reanudando el juego...");
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        Debug.Log("Reiniciando el juego...");
        Time.timeScale = 1f; // Asegúrate de que el tiempo esté activo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena
    }

    public void Cerrar()
    {
        Debug.Log("Cerrando el juego");
        Application.Quit();
    }
}
