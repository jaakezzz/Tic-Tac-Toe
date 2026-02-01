using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject gameCanvas;

    [Header("Panels")]
    public GameObject sideSelectionPanel;

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
        // Set both players to Human
        TicTacToeController.playerXType = PlayerType.Human;
        TicTacToeController.playerOType = PlayerType.Human;

        StartGame();
    }

    // Opens the panel where we choose X, O, or Watch AI
    public void OnPvAI_OpenPanel()
    {
        sideSelectionPanel.SetActive(true);
    }

    // 2. Player vs AI (Player is X)
    public void OnChooseX()
    {
        TicTacToeController.playerXType = PlayerType.Human;
        TicTacToeController.playerOType = PlayerType.AI;

        sideSelectionPanel.SetActive(false);
        StartGame();
    }

    // 3. Player vs AI (Player is O)
    public void OnChooseO()
    {
        TicTacToeController.playerXType = PlayerType.AI;
        TicTacToeController.playerOType = PlayerType.Human;

        sideSelectionPanel.SetActive(false);
        StartGame();
    }

    // 4. AI vs AI (Watcher Mode)
    public void OnAIvsAI()
    {
        TicTacToeController.playerXType = PlayerType.AI;
        TicTacToeController.playerOType = PlayerType.AI;

        sideSelectionPanel.SetActive(false);
        StartGame();
    }

    public void OnCancelSideSelection()
    {
        sideSelectionPanel.SetActive(false);
    }

    // --- SYSTEM FUNCTIONS ---

    void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    public void OnSettingsButton()
    {
        Debug.Log("Settings clicked! (To be implemented)");
    }

    public void OnQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}