using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public const int columns = 4;
    public const int rows = 2;
    public const float xspace = 4f;
    public const float yspace = -5f;

    [SerializeField] private MainImagesScript startObject;
    [SerializeField] private Sprite[] images;

    private MainImagesScript firstOpen;
    private MainImagesScript secondOpen;

    private int score = 0;
    private int attempts = 0;
    private const int maxAttempts = 8;  // Máximo de intentos permitidos

    [SerializeField] private TextMesh scoreText;
    [SerializeField] private TextMesh attemptsText;

    private int[] Randomiser(int[] locations)
    {
        int[] array = (int[])locations.Clone();
        for (int i = 0; i < array.Length; i++)
        {
            int newArray = array[i];
            int j = Random.Range(i, array.Length);
            array[i] = array[j];
            array[j] = newArray;
        }
        return array;
    }

    private void Start()
    {
        if (startObject == null)
        {
            Debug.LogError("startObject no está asignado. Por favor, asigna un objeto en el Inspector.");
            return;
        }
        if (scoreText == null || attemptsText == null)
        {
            Debug.LogError("Por favor, asigna los objetos de texto de puntaje e intentos en el Inspector.");
            return;
        }
        if (images == null || images.Length == 0)
        {
            Debug.LogError("No se han asignado imágenes en el array 'images'. Por favor, asigna las imágenes en el Inspector.");
            return;
        }

        int[] locations = { 0, 0, 1, 1, 2, 2, 3, 3 };
        locations = Randomiser(locations);

        Vector3 startPosition = startObject.transform.position;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                MainImagesScript gameImage;
                if (i == 0 && j == 0)
                {
                    gameImage = startObject;
                }
                else
                {
                    gameImage = Instantiate(startObject) as MainImagesScript;
                }

                int index = j * columns + i;
                int id = locations[index];
                gameImage.ChangeSprite(id, images[id]);

                float positionX = (xspace * i) + startPosition.x;
                float positionY = (yspace * j) + startPosition.y;
                gameImage.transform.position = new Vector3(positionX, positionY, startPosition.z);

                Debug.Log($"Imagen creada en la posición: {positionX}, {positionY}");
            }
        }
    }

    public bool canOpen
    {
        get { return secondOpen == null; }
    }

    public void ImageOpen(MainImagesScript selectedImage)
    {
        if (firstOpen == null)
        {
            firstOpen = selectedImage;
        }
        else
        {
            secondOpen = selectedImage;
            StartCoroutine(CheckGuessed());
        }
    }

    private IEnumerator CheckGuessed()
    {
        if (firstOpen.SpriteId == secondOpen.SpriteId)
        {
            score++;
            scoreText.text = "Score: " + score;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            firstOpen.Close();
            secondOpen.Close();
        }

        firstOpen = null;
        secondOpen = null;

        attempts++;
        attemptsText.text = "Attempts: " + attempts;

        // Verifica si se han alcanzado los intentos máximos
        if (attempts >= maxAttempts)
        {
            Debug.Log("Juego terminado. Se alcanzaron los intentos máximos.");
            // Reinicia la escena o toma alguna acción cuando se alcanzan los intentos
            Restart();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }
}
