using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject gameCanvas;

    [Header("Panels")]
    public GameObject sideSelectionPanel;
    public GameObject settingsPanel;

    void Start()
    {
        // Ensure menu is on and game is off when we hit Play
        mainMenuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        sideSelectionPanel.SetActive(false);
    }

    // --- BUTTON FUNCTIONS ---

    // 1. Player vs Player
    public void OnPvPButton()
    {
        AudioManager.Instance.PlayClick();
        // Set both players to Human
        TicTacToeController.playerXType = PlayerType.Human;
        TicTacToeController.playerOType = PlayerType.Human;

        StartGame();
    }

    // Opens the panel where we choose X, O, or Watch AI
    public void OnPvAI_OpenPanel()
    {
        AudioManager.Instance.PlayClick();
        sideSelectionPanel.SetActive(true);
    }

    // 2. Player vs AI (Player is X)
    public void OnChooseX()
    {
        AudioManager.Instance.PlayClick();
        TicTacToeController.playerXType = PlayerType.Human;
        TicTacToeController.playerOType = PlayerType.AI;

        sideSelectionPanel.SetActive(false);
        StartGame();
    }

    // 3. Player vs AI (Player is O)
    public void OnChooseO()
    {
        AudioManager.Instance.PlayClick();
        TicTacToeController.playerXType = PlayerType.AI;
        TicTacToeController.playerOType = PlayerType.Human;

        sideSelectionPanel.SetActive(false);
        StartGame();
    }

    // 4. AI vs AI (Watcher Mode)
    public void OnAIvsAI()
    {
        AudioManager.Instance.PlayClick();
        TicTacToeController.playerXType = PlayerType.AI;
        TicTacToeController.playerOType = PlayerType.AI;

        sideSelectionPanel.SetActive(false);
        StartGame();
    }

    public void OnBackToMain()
    {
        AudioManager.Instance.PlayClick();
        if (sideSelectionPanel) sideSelectionPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void OnSettingsButton()
    {
        AudioManager.Instance.PlayClick();
        settingsPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        // 1. Play the sound immediately
        AudioManager.Instance.PlayClick();

        // 2. Vaporize the mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 3. Nuke the Event System (guarantees no UI can be interacted with)
        UnityEngine.EventSystems.EventSystem.current.enabled = false;

        // 4. Delay the actual quit command to allow the sound to play
        Invoke("ExecuteQuit", 0.35f);
    }

    // --- SYSTEM FUNCTIONS ---

    void StartGame()
    {
        AudioManager.Instance.PlayGameMusic(); // SWITCH MUSIC
        mainMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    private void ExecuteQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}