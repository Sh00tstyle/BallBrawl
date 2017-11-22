using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public static bool isPaused = false;

    public static void PauseGame() {
        Debug.Log("Game paused");
        Time.timeScale = 0f;
        isPaused = true;
    }

    public static void ResumeGame() {
        Debug.Log("Game resumed");
        Time.timeScale = 1f;
        isPaused = false;
    }
}
