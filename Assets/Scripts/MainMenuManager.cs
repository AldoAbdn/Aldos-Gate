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
    public GameObject PlayerFourInputField;
    public GameObject PlayerFiveInputField;
    public TextMeshProUGUI PlayerOneText;
    public TextMeshProUGUI PlayerTwoText;
    public TextMeshProUGUI PlayerThreeText;
    public TextMeshProUGUI PlayerFourText;
    public TextMeshProUGUI PlayerFiveText;
    public int NumberOfPlayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BackToMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumberOfPlayers(int value)
    {
        NumberOfPlayers = NumberOfPlayersDropdown.value + 3; // Add 3 to the dropdown value to get the actual number of players
        // Disable player text fields based on number of players
        if (NumberOfPlayers < 4)
        {
            PlayerFourInputField.SetActive(false);
        }
        else
        {
            PlayerFourInputField.SetActive(true);
        }


        if (NumberOfPlayers < 5)
        {
            PlayerFiveInputField.SetActive(false);
        } 
        else
        {
            PlayerFiveInputField.SetActive(true);
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
        ResetPlayerSelect();
    }

    public void LoadGame()
    {
        List<string> playerNames = new List<string>();
        playerNames.Clear();
        playerNames.Add(PlayerOneText.text);
        playerNames.Add(PlayerTwoText.text);
        playerNames.Add(PlayerThreeText.text);
        if (NumberOfPlayers >= 4)
        {
            playerNames.Add(PlayerFourText.text);
        }
        if (NumberOfPlayers >= 5)
        {
            playerNames.Add(PlayerFiveText.text);
        }
        NavigationManager.NavigateToGameScene(playerNames, NumberOfPlayers);
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
        // Set player input fields to active
        PlayerFourInputField.SetActive(true);
        PlayerFiveInputField.SetActive(true);
        // Set number of players to 5
        NumberOfPlayers = 5;
        // Set dropdown value to 5 players
        NumberOfPlayersDropdown.value = 2; // Set to 2 because the dropdown value starts at 0 for 3 players
    }
}
