using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void NavigateToGameScene(List<string> playerNames, int numPlayers)
    {
        for (int i = 1; i <= 5; i++)
        {
            if (i <= numPlayers)
            {
                PlayerPrefs.SetString($"PlayerName{i}", playerNames[i-1]);
            }
            else
            {
                PlayerPrefs.SetString($"PlayerName{i}", string.Empty);
            }
        }

        SceneManager.LoadScene("AldosGate2DScene");
    }

    public void NavigateToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void NavigateToGameOverScene(string playerName, int playerType)
    {
        // Pass a string value with the players name to the Game Over scene
        // You can use PlayerPrefs to store the player's name and retrieve it in the Game Over scene
        PlayerPrefs.SetString("PlayerName", playerName);

        // Pass the type of player 
        PlayerPrefs.SetInt("PlayerType", playerType);

        SceneManager.LoadScene("GameOverScene");
    }
}
