using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public Game game;
    public GameUIView uiView;

    private float timer = 0f;
    private bool isPlaying = false;

    private GameSettings[] gameModes =
    {
        new GameSettings(8,8,10), //Easy
        new GameSettings(12,12,40), //Medium
        new GameSettings(16,16,90), //Hard
    };

    private void Start()
    {
        uiView.gameModeDropDown.onValueChanged.AddListener(OnGameModeChanged);
        uiView.restartButton.onClick.AddListener(RestartGame);
        OnGameModeChanged(0); //Default to easy
    }
    private void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;
            uiView.SetTimer(timer);
        }
    }

    private void RestartGame()
    {
        timer = 0;
        isPlaying = true;
        uiView.SetStatus("");
        game.StartNewGame(OnGameWin, OnGameLose);
    }

    private void OnGameModeChanged(int index)
    {
        GameSettings settings = gameModes[index];
        game.SetSettings(settings.width, settings.height, settings.mineCount);
        uiView.SetMinesCount(settings.mineCount);
        RestartGame();
    }

    private void OnGameWin()
    {
        isPlaying = false;
        uiView.SetStatus("YOU WIN!");
        AudioManager.Instance.PlayWin();
    }

    private void OnGameLose()
    {
        isPlaying = false;
        uiView.SetStatus("GAME OVER!");
        AudioManager.Instance.PlayExplosion();
        AudioManager.Instance.PlayLose();
    }
}
