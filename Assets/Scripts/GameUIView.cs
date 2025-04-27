using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUIView : MonoBehaviour
{
    public TMP_Dropdown gameModeDropDown;
    public Button restartButton;
    public TMP_Text timerText;
    public TMP_Text statusText;
    public TMP_Text minesCountText;

    public void SetStatus(string message)
    {
        statusText.text = message;
    }

    public void SetMinesCount(int minesCount)
    {
        minesCountText.text = $"{minesCount}";
    }

    public void SetTimer(float time)
    {
        timerText.text = $"{time:F0}s";
    }
}
