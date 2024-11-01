using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public const int columnsLevel1 = 4; // Columnas del nivel 1
    public const int rowsLevel1 = 2;    // Filas del nivel 1
    public const float xspace = 4f;      // Espacio en el eje X
    public const float yspace = -5f;     // Espacio en el eje Y

    public const int columnsLevel2 = 5; // Columnas del nivel 2
    public const int rowsLevel2 = 2;    // Filas del nivel 2
    private const int maxAttemptsLevel1 = 8; // Máximo de intentos nivel 1
    private const int maxAttemptsLevel2 = 30000; // Máximo de intentos nivel 2

    public GameObject gameOverPanel; 
    public GameObject winPanel;
    [SerializeField] private MainImagesScript startObject;
    [SerializeField] private Sprite[] level1Images; // Imágenes para el primer nivel
    [SerializeField] private Sprite[] level2Images; // Imágenes para el segundo nivel
    [SerializeField] private TextMeshProUGUI finalScoreText; 
    [SerializeField] private TextMeshProUGUI finalAttemptsText;

    private MainImagesScript firstOpen;
    private MainImagesScript secondOpen;
    private string sceneToLoad = "Menu";
    private string siguienteEscena = "SegundoNivel";

    private int score = 0;
    private int attempts = 0;
    private int maxAttempts; // Variable para el máximo de intentos
    private int totalCards; // Declarar totalCards como variable de instancia

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private AudioSource matchSound;  // Referencia al AudioSource para el sonido de coincidencia

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

        // Inicializa el juego basándose en el nivel actual
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Establecer columnas y filas según el nivel
        int columns = (SceneManager.GetActiveScene().name == "SegundoNivel") ? columnsLevel2 : columnsLevel1;
        int rows = (SceneManager.GetActiveScene().name == "SegundoNivel") ? rowsLevel2 : rowsLevel1;

        maxAttempts = (SceneManager.GetActiveScene().name == "SegundoNivel") ? maxAttemptsLevel2 : maxAttemptsLevel1; // Establece el máximo de intentos

        Sprite[] currentImages = (SceneManager.GetActiveScene().name == "SegundoNivel") ? level2Images : level1Images;

        // Verifica que hay imágenes disponibles
        if (currentImages == null || currentImages.Length == 0)
        {
            Debug.LogError("No se han asignado imágenes en el array 'currentImages'. Por favor, asigna las imágenes en el Inspector.");
            return;
        }

        totalCards = columns * rows; // Total de cartas

        // Asegúrate de que el número de cartas sea par
        totalCards += totalCards % 2; // Redondea hacia arriba para tener un número par de cartas

        if (totalCards / 2 > currentImages.Length)
        {
            Debug.LogError("No hay suficientes imágenes para crear el juego. Asegúrate de tener al menos " + (totalCards / 2) + " imágenes.");
            return; // Salir si no hay suficientes imágenes
        }

        List<int> imageIndices = new List<int>();
        for (int i = 0; i < currentImages.Length; i++)
        {
            imageIndices.Add(i);
        }

        // Selecciona imágenes aleatorias, asegurando pares
        int[] locations = new int[totalCards];
        for (int i = 0; i < totalCards / 2; i++)
        {
            int randomIndex = Random.Range(0, imageIndices.Count);
            int imageIndex = imageIndices[randomIndex];

            locations[2 * i] = imageIndex;      // Primer uso
            locations[2 * i + 1] = imageIndex;  // Segundo uso

            imageIndices.RemoveAt(randomIndex); // Elimina la imagen utilizada para no repetir
        }

        locations = Randomiser(locations); // Aleatoriza posiciones

        Vector3 startPosition = startObject.transform.position; // Posición inicial

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                MainImagesScript gameImage;
                if (i == 0 && j == 0)
                {
                    gameImage = startObject; // Usa el objeto base
                }
                else
                {
                    gameImage = Instantiate(startObject) as MainImagesScript; // Crea una nueva carta
                }

                int index = j * columns + i; // Índice en el array
                int id = locations[index]; // ID de la imagen

                // Asegúrate de que el índice ID no exceda la longitud del array
                if (id < currentImages.Length)
                {
                    gameImage.ChangeSprite(id, currentImages[id]); // Cambia la imagen de la carta
                }
                else
                {
                    Debug.LogError("Índice de imagen fuera de rango: " + id);
                    continue; // Salta si el índice es incorrecto
                }

                // Calcula posición de la carta
                float positionX = (xspace * i) + startPosition.x;
                float positionY = (yspace * j) + startPosition.y;
                gameImage.transform.position = new Vector3(positionX, positionY, startPosition.z);
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
            scoreText.text = "Puntuacion: " + score;

            // Reproduce el sonido de coincidencia
            if (matchSound != null)
            {
                matchSound.Play();
            }
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
        attemptsText.text = "Intentos: " + attempts;

        // Verifica si se han alcanzado los intentos máximos
        if (score >= (totalCards / 2)) // Ajusta la condición para la victoria
        {
            WinPanel();
        }

        if (attempts >= maxAttempts)
        {
            ShowGameOver();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0; // Pausa el juego
        gameOverPanel.SetActive(true); // Muestra el menú de Game Over
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void WinPanel()
    {
        Time.timeScale = 0;
        winPanel.SetActive(true);
        finalScoreText.text = "Puntuación: " + score;
        finalAttemptsText.text = "Intentos: " + attempts;
    }

    public void Siguiente()
    {
        SceneManager.LoadScene(siguienteEscena);
    }

    public void SalirDeLaApp()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
