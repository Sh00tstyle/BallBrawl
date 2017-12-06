using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManagerScript : MonoBehaviour {

    private static UiManagerScript _instance;

    [SerializeField]
    private GameObject _pauseMenu;

    public void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        QualitySettings.vSyncCount = 0; //No vsync
    }

    public void ActivatePauseMenu() {
        if (_pauseMenu.activeSelf) return;
        _pauseMenu.SetActive(true);

        Time.timeScale = 0f;
    }

    public void DeactivatePauseMenu() {
        if (!_pauseMenu.activeSelf) return;
        _pauseMenu.SetActive(false);

        Time.timeScale = 1f;
    }

    public void HidePauseMenu() {
        DeactivatePauseMenu();

        PauseHandlerScript[] pauseHelper = FindObjectsOfType<PauseHandlerScript>();

        for(int i = 0; i < pauseHelper.Length; i++) {
            try {
                pauseHelper[i].CmdSetPause(false);
            } catch { }
        }


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static UiManagerScript Instance {
        get { return _instance; }
    }
}
