using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonPlay : MonoBehaviour {
    public int LoadingLevel = 0;

    public GameObject confirm_menu;
    public Button playbutton;
    public Button quitbutton;

    public void OnPlayPressed()
    {
        SceneManager.LoadScene(LoadingLevel);
    }

    public void OnQuitPressed()
    {
        playbutton.interactable = false;
        quitbutton.interactable = false;
        confirm_menu.SetActive(true);
    }

    public void OnQuitConfirm()
    {
        Application.Quit();
    }

    public void OnQuitNotConfirm()
    {
        playbutton.interactable = true;
        quitbutton.interactable = true;
        confirm_menu.SetActive(false);
    }
}
