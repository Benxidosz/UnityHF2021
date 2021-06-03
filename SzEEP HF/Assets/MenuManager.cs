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
    
    private Animator _mainMenuAnim;
    private Animator _chooserAnim;

    private void Awake() {
        _intance = this;
        _mainMenuAnim = mainMenu.GetComponent<Animator>();
        _chooserAnim = characterChooser.GetComponent<Animator>();
    }

    public void StartGamePlayerVsPlayer() {
        startingFlags.playerVsPlayer = true;
        SceneManager.LoadScene(1);
    }

    public void PlayerVsBot() {
        startingFlags.playerVsPlayer = false;
        StartCoroutine(Switch());
    }

    public void ChooseCat() {
        startingFlags.playerIsCat = true;
        SceneManager.LoadScene(1);
    }

    public void ChooseDog() {
        startingFlags.playerIsCat = false;
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        Application.Quit();
    }
    
    IEnumerator Switch() {
        _mainMenuAnim.SetTrigger("Switch");
        _chooserAnim.SetTrigger("Switch");
        yield return new WaitForSeconds(1);
    }
}
