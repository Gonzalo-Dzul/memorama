using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImagesScript : MonoBehaviour
{
    [SerializeField] private GameObject image_unKnown;
    [SerializeField] private GameController gameController;
    [SerializeField] private AudioSource audioSource; // Referencia al AudioSource

    private int _spriteId;
    public int SpriteId
    {
        get { return _spriteId; }
    }

    private void OnMouseDown()
    {
        if (image_unKnown.activeSelf && gameController.canOpen) 
        {
            image_unKnown.SetActive(false);
            audioSource.Play(); // Reproduce el sonido al seleccionar la carta
            gameController.ImageOpen(this);
        }
    }

    public void ChangeSprite(int id, Sprite image)
    {
        _spriteId = id;
        GetComponent<SpriteRenderer>().sprite = image;
    }

    // Método para voltear la carta hacia arriba
    public void Open()
    {
        image_unKnown.SetActive(false); // Muestra la imagen
    }

    public void Close()
    {
        image_unKnown.SetActive(true);
    }
}
