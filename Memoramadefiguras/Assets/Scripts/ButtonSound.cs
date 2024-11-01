using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    public void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Reproducir el sonido
        }
        else
        {
            Debug.LogWarning("AudioSource no está asignado en el objeto: " + gameObject.name);
        }
    }
}
