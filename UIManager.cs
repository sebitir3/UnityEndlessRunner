using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Game Screen")]
    public GameObject gameScreen;
    public TMP_Text gameScoreText;
    public ulong currentScore;
    public int scoreRate = 100;

    [Header("Highscore Screen")]
    public GameObject hsScreen;

    void Start()
    {
        currentScore = 0;
    }

    void Update()
    {
        currentScore += (ulong)(scoreRate * Time.deltaTime);
        gameScoreText.text = $"Score: {currentScore}";
    }
}
