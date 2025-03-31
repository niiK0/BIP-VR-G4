using UnityEngine;
using UnityEngine.UI; // Para manipular a UI
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText; // Arraste o objeto de texto da UI no Inspector
    private int score = 0; // Inicializa a pontuação

    private void Awake()
    {
        ScoreManager.Instance = this;
    }

    void Start()
    {
        UpdateScoreUI(); // Atualiza a UI no início
    }

    public void AddScore(int points)
    {
        score += points; // Adiciona pontos
        UpdateScoreUI(); // Atualiza a UI
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score; // Exibe os pontos na tela
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
