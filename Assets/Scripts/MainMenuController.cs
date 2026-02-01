using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject gameCanvas;

    public static bool isVsAI = false;
    public static string playerSide = "X"; // Defaults to X

    [Header("Panels")]
    public GameObject sideSelectionPanel;

    void Start()
    {
        // Ensure menu is on and game is off when we hit Play
        mainMenuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
    }

    public void OnPvPButton()
    {
        isVsAI = false;
        StartGame();
    }

    public void OnPvAI_OpenPanel()
    {
        sideSelectionPanel.SetActive(true);
    }

    public void OnChooseX()
    {
        isVsAI = true;
        playerSide = "X";
        sideSelectionPanel.SetActive(false); // Hide panel
        StartGame();
    }

    public void OnChooseO()
    {
        isVsAI = true;
        playerSide = "O";
        sideSelectionPanel.SetActive(false); // Hide panel
        StartGame();
    }

    public void OnCancelSideSelection()
    {
        sideSelectionPanel.SetActive(false);
    }

    void StartGame()
    {
        // Swap the canvases
        mainMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        // TODO: Inform the GameController to reset the board
    }

    public void OnSettingsButton()
    {
        Debug.Log("Settings clicked! (To be implemented)");
    }

    public void OnQuitButton()
    {
        Application.Quit();

        // This allows us to quit even when testing in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}