using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _intance;
    public static MenuManager Instance => _intance;

    public StartingFlagsSO startingFlags;
    public GameObject mainMenu;
    public GameObject characterChooser;

    private void Awake() {
        _intance = this;
        mainMenu.SetActive(true);
        characterChooser.SetActive(false);
    }

    public void StartGamePlayerVsPlayer() {
        startingFlags.playerVsPlayer = true;
        SceneManager.LoadScene(1);
    }

    public void PlayerVsBot() {
        mainMenu.SetActive(false);
        characterChooser.SetActive(true);
        startingFlags.playerVsPlayer = false;
    }

    public void ChooseCat() {
        startingFlags.playerIsCat = true;
        SceneManager.LoadScene(1);
    }

    public void ChooseDog() {
        startingFlags.playerIsCat = false;
        SceneManager.LoadScene(1);
    }
}
