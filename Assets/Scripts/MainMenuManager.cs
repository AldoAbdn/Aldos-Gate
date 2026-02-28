using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Canvas NewGameCanvas;
    public Canvas PlayerSelectCanvas;
    public NavigationManager NavigationManager;
    public TMP_Dropdown NumberOfPlayersDropdown;
    public TextMeshProUGUI PlayerOneText;
    public TextMeshProUGUI PlayerTwoText;
    public TextMeshProUGUI PlayerThreeText;
    public TextMeshProUGUI PlayerFourText;
    public TextMeshProUGUI PlayerFiveText;
    public int NumberOfPlayers;
    private List<string> PlayerNames = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BackToMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumberOfPlayers()
    {
        NumberOfPlayers = NumberOfPlayersDropdown.value;
        // Disable player text fields based on number of players
        if (NumberOfPlayers < 4)
        {
            PlayerFourText.gameObject.SetActive(false);
        }
        else
        {
            PlayerFourText.gameObject.SetActive(true);
        }


        if (NumberOfPlayers < 5)
        {
            PlayerFiveText.gameObject.SetActive(false);
        } 
        else
        {
            PlayerFiveText.gameObject.SetActive(true);
        }
    }

    public void NewGame()
    {
        NewGameCanvas.gameObject.SetActive(false);
        PlayerSelectCanvas.gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        NewGameCanvas.gameObject.SetActive(true);
        PlayerSelectCanvas.gameObject.SetActive(false);
        Console.WriteLine("Test");
        ResetPlayerSelect();
        Console.WriteLine("Test2");
    }

    public void LoadGame()
    {
        PlayerNames.Clear();
        PlayerNames.Add(PlayerOneText.text);
        PlayerNames.Add(PlayerTwoText.text);
        PlayerNames.Add(PlayerThreeText.text);
        if (NumberOfPlayers >= 4)
        {
            PlayerNames.Add(PlayerFourText.text);
        }
        if (NumberOfPlayers >= 5)
        {
            PlayerNames.Add(PlayerFiveText.text);
        }
        NavigationManager.NavigateToGameScene(PlayerNames, NumberOfPlayers);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ResetPlayerSelect()
    {
        // Clear player names list
        PlayerOneText.text = "";
        PlayerTwoText.text = "";
        PlayerThreeText.text = "";
        PlayerFourText.text = "";
        PlayerFiveText.text = "";
        // Set player text fields to enabled true
        PlayerOneText.gameObject.SetActive(true);
        PlayerTwoText.gameObject.SetActive(true);
        PlayerThreeText.gameObject.SetActive(true);
        PlayerFourText.gameObject.SetActive(true);
        PlayerFiveText.gameObject.SetActive(true);
        // Set number of players to 5
        NumberOfPlayers = 5;
    }
}
