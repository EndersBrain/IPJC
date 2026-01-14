using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Debug cheat controller for development/testing.
/// Uses numpad keys for various debug functions.
/// 
/// Controls:
///   Numpad 7 - Toggle God Mode (constant healing)
///   Numpad 8 - Previous scene (by build index)
///   Numpad 9 - Next scene (by build index)
///   Numpad 5 - Pause game (Time.timeScale = 0)
///   Numpad 6 - Resume game (Time.timeScale = 1)
/// </summary>
public class DebugCheatController : MonoBehaviour
{
    [Header("God Mode")]
    
    private bool m_godModeActive = false;
    private StatController m_playerStats;
    
    private GUIStyle m_labelStyle;
    private bool m_isPaused = false;
    
    void Start()
    {
        // Find player stats
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            m_playerStats = player.GetComponent<StatController>();
            if (m_playerStats == null) {
                m_playerStats = player.GetComponentInChildren<StatController>();
            }
        }
        
        // Try finding by component if tag didn't work
        if (m_playerStats == null) {
            var playerController = FindFirstObjectByType<PlayerControllerClean>();
            if (playerController != null) {
                m_playerStats = playerController.GetComponent<StatController>();
            }
        }
    }
    
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        // Numpad 7 - Toggle God Mode
        if (keyboard.numpad7Key.wasPressedThisFrame) {
            ToggleGodMode();
        }
        
        // Numpad 8 - Previous Scene
        if (keyboard.numpad8Key.wasPressedThisFrame) {
            LoadPreviousScene();
        }
        
        // Numpad 9 - Next Scene
        if (keyboard.numpad9Key.wasPressedThisFrame) {
            LoadNextScene();
        }
        
        // Numpad 5 - Pause
        if (keyboard.numpad5Key.wasPressedThisFrame) {
            PauseGame();
        }
        
        // Numpad 6 - Resume
        if (keyboard.numpad6Key.wasPressedThisFrame) {
            ResumeGame();
        }
        
        // Apply god mode healing
        if (m_godModeActive && m_playerStats != null) {
            m_playerStats.SetResourceToMax(StatType.Health);
        }
    }
    
    private void ToggleGodMode()
    {
        m_godModeActive = !m_godModeActive;
        Debug.Log($"God Mode: {(m_godModeActive ? "ON" : "OFF")}");
    }
    
    private void LoadPreviousScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int prevIndex = currentIndex - 1;
        
        if (prevIndex < 0) {
            prevIndex = SceneManager.sceneCountInBuildSettings - 1;
        }
        
        Debug.Log($"Loading previous scene: {prevIndex}");
        SceneManager.LoadScene(prevIndex);
    }
    
    private void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        
        if (nextIndex >= SceneManager.sceneCountInBuildSettings) {
            nextIndex = 0;
        }
        
        Debug.Log($"Loading next scene: {nextIndex}");
        SceneManager.LoadScene(nextIndex);
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0f;
        m_isPaused = true;
        Debug.Log("Game Paused");
    }
    
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        m_isPaused = false;
        Debug.Log("Game Resumed");
    }
    
    void OnGUI()
    {
        // Show status in top-left corner
        if (!m_godModeActive && !m_isPaused) return;
        
        if (m_labelStyle == null) {
            m_labelStyle = new GUIStyle(GUI.skin.label);
            m_labelStyle.fontSize = 14;
            m_labelStyle.fontStyle = FontStyle.Bold;
        }
        
        float y = 10;
        
        if (m_godModeActive) {
            m_labelStyle.normal.textColor = Color.green;
            GUI.Label(new Rect(10, y, 200, 25), "GOD MODE", m_labelStyle);
            y += 20;
        }
        
        if (m_isPaused) {
            m_labelStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(10, y, 200, 25), "PAUSED", m_labelStyle);
        }
    }
}
