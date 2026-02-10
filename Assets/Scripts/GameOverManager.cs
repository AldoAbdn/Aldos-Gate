using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerTypeText;

    void Update()
    {
        PlayerNameText.text = PlayerPrefs.GetString("PlayerName", "Unknown Player");
        int playerType = PlayerPrefs.GetInt("PlayerType", 0);
        GameManager.RoundWinner roundWinner = (GameManager.RoundWinner)playerType;
        if (playerType == -1)
        {
            PlayerTypeText.text = "It's a Draw!";
            return;
        }
        string playerTypeString = roundWinner.ToString();
        PlayerTypeText.text = $"{playerTypeString} Won the Game";
    }
}
