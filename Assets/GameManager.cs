using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameWonCanvas;

    private void Start()
    {
        StartGame();
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += HandlePlayerDeath;
    }
    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;
    }

    public void StartGame()
    {
        menuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        gameWonCanvas.SetActive(false);
        //Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        menuCanvas.SetActive(false);
        //Time.timeScale = 1f;
    }

    private void HandlePlayerDeath()
    {
        //game over and after 5 seconds, show menu to play or exit
        StartCoroutine(ShowGameOverMenu());
    }

    private System.Collections.IEnumerator ShowGameOverMenu()
    {
        yield return new WaitForSecondsRealtime(2f);
        gameOverCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(5f);
        gameOverCanvas.SetActive(false);
        Retry();
    }

    public void Retry()
    {
        StartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TestButton()
{
    Debug.Log("Botó clicat!");
}
}
