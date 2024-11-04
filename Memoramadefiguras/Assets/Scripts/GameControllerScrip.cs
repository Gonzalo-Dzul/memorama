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

    // Intentos base para cada nivel
    private const int baseAttemptsLevel1 = 8;
    private const int baseAttemptsLevel2 = 13;
    private const int baseAttemptsLevel3 = 20;

    private int totalAttempts = 0;

    private bool gameEnded = false; // Nueva variable para evitar múltiples paneles activos

    public GameObject gameOverPanel;
    public GameObject winPanel;
    [SerializeField] private MainImagesScript startObject;
    [SerializeField] private Sprite[] level1Images;
    [SerializeField] private Sprite[] level2Images;
    [SerializeField] private Sprite[] level3Images; // Imágenes para el tercer nivel
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalAttemptsText;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    private MainImagesScript firstOpen;
    private MainImagesScript secondOpen;
    private string sceneToLoad = "Menu";
    private string siguienteEscena = "SegundoNivel";
    private string tercerEscena = "TercerNivel"; // Nombre de la escena del tercer nivel

    private int score = 0;
    private int remainingAttempts;
    private int totalCards;

    private float startTime; // Variable para almacenar el tiempo de inicio
    private bool isGameActive = false; // Controla si el juego está activo

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private AudioSource matchSound;
    [SerializeField] private AudioSource errorSound; // Sonido de error
    [SerializeField] private AudioSource winSound;   // Sonido de ganar

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

        scoreText.text = "Puntuación: " + score;

        // Configura el nivel según la escena actual
        int columns, rows;
        Sprite[] currentImages;
        float currentXSpace = xspace;
        float currentYSpace = yspace;

        if (SceneManager.GetActiveScene().name == "SegundoNivel")
        {
            columns = columnsLevel2;
            rows = rowsLevel2;
            remainingAttempts = baseAttemptsLevel2;
            currentImages = level2Images;
        }
        else if (SceneManager.GetActiveScene().name == "TercerNivel")
        {
            columns = columnsLevel3;
            rows = rowsLevel3;
            remainingAttempts = baseAttemptsLevel3;
            currentImages = level3Images;
            currentXSpace = 3f;
            currentYSpace = -5f;
        }
        else
        {
            columns = columnsLevel1;
            rows = rowsLevel1;
            remainingAttempts = baseAttemptsLevel1;
            currentImages = level1Images;
        }

        totalCards = columns * rows;
        totalCards += totalCards % 2;

        if (totalCards / 2 > currentImages.Length)
        {
            Debug.LogError("No hay suficientes imágenes para crear el juego.");
            return;
        }

        attemptsText.text = "Intentos: " + remainingAttempts;

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
                MainImagesScript gameImage = (i == 0 && j == 0) ? startObject : Instantiate(startObject);
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

                float positionX = isThirdLevel ? (currentXSpace * j) + startPosition.x : (xspace * i) + startPosition.x;
                float positionY = isThirdLevel ? (currentYSpace * i) + startPosition.y : (yspace * j) + startPosition.y;
                gameImage.transform.position = new Vector3(positionX, positionY, startPosition.z);
            }
        }

        startTime = Time.time;
        isGameActive = true;
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
        if (gameEnded) yield break; // No ejecutar si el juego ya ha terminado
        remainingAttempts--;
        totalAttempts++;
        attemptsText.text = "Intentos: " + remainingAttempts;

        if (firstOpen.SpriteId == secondOpen.SpriteId)
        {
            score++;
            scoreText.text = "Puntuación: " + score;

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

            if (errorSound != null)
            {
                errorSound.Play();
            }
        }

        firstOpen = null;
        secondOpen = null;

        if (score >= (totalCards / 2))
        {
            WinPanel();
        }

        if (remainingAttempts <= 0)
        {
            ShowGameOver();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        isGameActive = false;
        gameEnded = false;
        firstOpen = null;
        secondOpen = null;
        score = 0;
        totalAttempts = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0;
        isGameActive = false;
        gameOverPanel.SetActive(true);
    }

    public void LoadScene()
    {
        // Restablecemos el estado del juego
        isGameActive = false;
        firstOpen = null;
        secondOpen = null;
        score = 0;
        totalAttempts = 0;
        remainingAttempts = 0;
        
        // Desactivar los paneles de victoria y derrota para evitar problemas de interfaz
        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // Cargar la escena del menú
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void Update()
    {
        // Actualizar cronómetro si el juego está activo
        if (isGameActive)
        {
            float elapsedTime = Time.time - startTime;
            // Muestra el tiempo transcurrido en la interfaz de usuario si es necesario
        }
    }

    public void WinPanel()
    {
         if (gameEnded) return; // Evitar activar múltiples paneles

        gameEnded = true;
        Time.timeScale = 0;
        isGameActive = false; // Desactivar el juego para que no se volteen más cartas
        winPanel.SetActive(true);
        finalScoreText.text = "Puntuación: " + score;
        finalAttemptsText.text = "Intentos realizados: " + totalAttempts;

        float finalTime = Time.time - startTime;
        finalTimeText.text = "Tiempo: " + finalTime.ToString("F2") + " seg.";

        if (winSound != null)
        {
            winSound.Play();
        }
    }

    public void Siguiente()
    {
        Time.timeScale = 1f;
        isGameActive = false; // Reinicia el estado del juego
        firstOpen = null;
        secondOpen = null;
        score = 0;
        totalAttempts = 0;
        remainingAttempts = 0;
        
        // Desactiva los paneles para el nuevo nivel
        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // Carga la escena correspondiente
        if (SceneManager.GetActiveScene().name == "SegundoNivel")
        {
            SceneManager.LoadScene(tercerEscena);
        }
        else
        {
            SceneManager.LoadScene(siguienteEscena);
        }
    }

    public void SalirDeLaApp()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
