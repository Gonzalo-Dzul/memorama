using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    // Columnas y filas para cada nivel
    public const int columnsLevel1 = 4;
    public const int rowsLevel1 = 2;
    public const int columnsLevel2 = 5;
    public const int rowsLevel2 = 2;
    public const int columnsLevel3 = 2; // Columnas del nivel 3
    public const int rowsLevel3 = 6;    // Filas del nivel 3
    public const float xspace = 4f;
    public const float yspace = -5f;

    // Intentos máximos para cada nivel
    private const int maxAttemptsLevel1 = 8;
    private const int maxAttemptsLevel2 = 30000;
    private const int maxAttemptsLevel3 = 20;

    public GameObject gameOverPanel; 
    public GameObject winPanel;
    [SerializeField] private MainImagesScript startObject;
    [SerializeField] private Sprite[] level1Images;
    [SerializeField] private Sprite[] level2Images;
    [SerializeField] private Sprite[] level3Images; // Imágenes para el tercer nivel
    [SerializeField] private TextMeshProUGUI finalScoreText; 
    [SerializeField] private TextMeshProUGUI finalAttemptsText;

    private MainImagesScript firstOpen;
    private MainImagesScript secondOpen;
    private string sceneToLoad = "Menu";
    private string siguienteEscena = "SegundoNivel";
    private string tercerEscena = "TercerNivel"; // Nombre de la escena del tercer nivel

    private int score = 0;
    private int attempts = 0;
    private int maxAttempts;
    private int totalCards;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private AudioSource matchSound;

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

        InitializeGame();
    }

    private void InitializeGame()
{
    firstOpen = null;
    secondOpen = null;
    score = 0;
    attempts = 0;

    scoreText.text = "Puntuacion: " + score;
    attemptsText.text = "Intentos: " + attempts;

    // Configura el nivel según la escena actual
    int columns, rows;
    Sprite[] currentImages;
    float currentXSpace = xspace;
    float currentYSpace = yspace;

    if (SceneManager.GetActiveScene().name == "SegundoNivel")
    {
        columns = columnsLevel2;
        rows = rowsLevel2;
        maxAttempts = maxAttemptsLevel2;
        currentImages = level2Images;
    }
    else if (SceneManager.GetActiveScene().name == "TercerNivel")
    {
        columns = columnsLevel3;
        rows = rowsLevel3;
        maxAttempts = maxAttemptsLevel3;
        currentImages = level3Images;

        // Ajusta el espaciado para el tercer nivel
        currentXSpace = 3f; // Reduce el espacio horizontal si es necesario
        currentYSpace = -5f; // Reduce el espacio vertical para que quepan más cartas
    }
    else
    {
        columns = columnsLevel1;
        rows = rowsLevel1;
        maxAttempts = maxAttemptsLevel1;
        currentImages = level1Images;
    }

    totalCards = columns * rows;
    totalCards += totalCards % 2;

    if (totalCards / 2 > currentImages.Length)
    {
        Debug.LogError("No hay suficientes imágenes para crear el juego. Asegúrate de tener al menos " + (totalCards / 2) + " imágenes.");
        return;
    }

    List<int> imageIndices = new List<int>();
    for (int i = 0; i < currentImages.Length; i++)
    {
        imageIndices.Add(i);
    }

    int[] locations = new int[totalCards];
    for (int i = 0; i < totalCards / 2; i++)
    {
        int randomIndex = Random.Range(0, imageIndices.Count);
        int imageIndex = imageIndices[randomIndex];

        locations[2 * i] = imageIndex;
        locations[2 * i + 1] = imageIndex;

        imageIndices.RemoveAt(randomIndex);
    }

    locations = Randomiser(locations);

    Vector3 startPosition = startObject.transform.position;

    bool isThirdLevel = SceneManager.GetActiveScene().name == "TercerNivel";

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

            if (id < currentImages.Length)
            {
                gameImage.ChangeSprite(id, currentImages[id]);
            }
            else
            {
                Debug.LogError("Índice de imagen fuera de rango: " + id);
                continue;
            }

            float positionX, positionY;

            if (isThirdLevel)
            {
                // Configuración vertical para el tercer nivel con ajuste de espacio
                positionX = (currentXSpace * j) + startPosition.x;
                positionY = (currentYSpace * i) + startPosition.y;
            }
            else
            {
                // Configuración estándar (horizontal)
                positionX = (xspace * i) + startPosition.x;
                positionY = (yspace * j) + startPosition.y;
            }

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

        if (score >= (totalCards / 2))
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
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
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
        Time.timeScale = 1f;
        
        // Cargar la escena correspondiente según el nivel completado
        if (SceneManager.GetActiveScene().name == "SegundoNivel")
        {
            SceneManager.LoadScene(tercerEscena); // Carga el tercer nivel
        }
        else
        {
            SceneManager.LoadScene(siguienteEscena); // Carga el segundo nivel
        }
    }

    public void SalirDeLaApp()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
