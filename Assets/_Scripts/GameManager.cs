using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameWonCanvas;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip loseSound;

    private void Awake()
    {
        StartCoroutine(ReloadGameScene());
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += HandlePlayerDeath;
        PlayerWin.OnPlayerWin += HandlePlayerWin;
    }
    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= HandlePlayerDeath;
        PlayerWin.OnPlayerWin -= HandlePlayerWin;
    }

    public void StartGame()
    {
        menuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        gameWonCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        menuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandlePlayerWin()
    {
        StartCoroutine(ShowGameWonMenu());
    }

    private System.Collections.IEnumerator ShowGameWonMenu()
    {
        gameWonCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(4f);
        gameWonCanvas.SetActive(false);
        Retry();
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(ShowGameOverMenu());
    }

    private System.Collections.IEnumerator ShowGameOverMenu()
    {
        yield return new WaitForSecondsRealtime(1f);
        audioSource.PlayOneShot(loseSound);
        gameOverCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(4f);
        gameOverCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Retry();
    }

    public void Retry()
    {
        StartCoroutine(ReloadGameScene());
    }

    private IEnumerator ReloadGameScene()
    {
        if (SceneManager.GetSceneByName("Game").isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync("Game");
        }

        yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");

        foreach (GameObject obj in cubes)
        {
            Destroy(obj);
        }

        StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
