using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource backgroundMusic;   // AudioSource para la música de fondo

    void Start()
    {
        // Reproduce la música de fondo en bucle
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }
}

