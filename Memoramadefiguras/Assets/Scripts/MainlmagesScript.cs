using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImagesScript : MonoBehaviour
{
    [SerializeField] private GameObject image_unKnown;
    [SerializeField] private GameController gameController; // Corregido el nombre del script

    // Este método se ejecuta cuando se hace clic sobre el objeto al que está adjunto este script
    private void OnMouseDown()
    {
        if (image_unKnown.activeSelf && gameController.canOpen) 
        {
            image_unKnown.SetActive(false);
            gameController.ImageOpen(this); // Corregido el nombre del método
        }
    }

    private int _spriteId;
    public int SpriteId // Corregido el nombre de la propiedad
    {
        get { return _spriteId; }
    }

    // Método para cambiar el sprite de la imagen
    public void ChangeSprite(int id, Sprite image)
    {
        _spriteId = id;
        GetComponent<SpriteRenderer>().sprite = image; // Cambiar el sprite a la nueva imagen.
    }

    // Método para cerrar la imagen y mostrar el estado desconocido
    public void Close()
    {
        image_unKnown.SetActive(true); // Ocultar la imagen
    }
}
