using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum PlayerType { Human, AI }

public class TicTacToeController : MonoBehaviour
{
    [Header("Game Settings")]
    public static PlayerType playerXType = PlayerType.Human;
    public static PlayerType playerOType = PlayerType.AI;

    [Header("UI References")]
    public GameObject[] gridButtons; // Drag all 9 buttons here in order
    public Image statusImage;
    public GameObject gameCanvas;
    public GameObject mainMenuCanvas;

    [Header("Status Sprites")]
    public Sprite xTurnSprite;
    public Sprite oTurnSprite;
    public Sprite xWinSprite;
    public Sprite oWinSprite;
    public Sprite drawSprite;

    [Header("Assets")]
    public Sprite xSprite;
    public Sprite oSprite;

    // Internal State
    private string[] boardState = new string[9]; // Tracks "X", "O", or ""
    private bool isPlayerXTurn = true;
    private bool gameActive = true;
    private int moveCount = 0;

    void Start()
    {
        // Auto-wire the buttons so we don't have to do it manually in Inspector
        for (int i = 0; i < gridButtons.Length; i++)
        {
            int index = i; // Local copy for the closure
            gridButtons[i].GetComponent<Button>().onClick.AddListener(() => OnCellClicked(index));
        }

        ResetGame();
    }

    // Called when the Main Menu starts the game
    private void OnEnable()
    {
        ResetGame();
    }

    public void OnCellClicked(int index, bool fromAI = false)
    {
        // If game is over or cell is already taken, ignore click
        if (!gameActive || boardState[index] != "") return;

        // Check if the current player is actually a Human
        PlayerType currentType = isPlayerXTurn ? playerXType : playerOType;

        // If it's an AI turn, but a Human clicked... BLOCK IT.
        if (currentType == PlayerType.AI && !fromAI)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        // 1. Update Internal Data
        string currentPlayer = isPlayerXTurn ? "X" : "O";
        boardState[index] = currentPlayer;
        moveCount++;

        // 2. Update Visuals
        // Get the Image component from the CHILD object
        Image btnImage = gridButtons[index].transform.GetChild(1).GetComponent<Image>();
        btnImage.sprite = (currentPlayer == "X") ? xSprite : oSprite;
        btnImage.color = Color.white; // Make the sprite fully visible

        gridButtons[index].GetComponent<Button>().interactable = false;

        // 3. Check Win Condition
        if (CheckWin(currentPlayer))
        {
            gameActive = false;
            statusImage.sprite = (currentPlayer == "X") ? xWinSprite : oWinSprite;
            HighlightWinLine(currentPlayer);

            // Disable all buttons so no more highlighting occurs
            foreach (GameObject btn in gridButtons)
            {
                btn.GetComponent<Button>().interactable = false;
            }
            return;
        }

        // 4. Check Draw
        if (moveCount >= 9)
        {
            gameActive = false;
            statusImage.sprite = drawSprite;

            // Disable all buttons so no more highlighting occurs
            foreach (GameObject btn in gridButtons)
            {
                btn.GetComponent<Button>().interactable = false;
            }
            return;
        }

        // 5. Switch Turn
        isPlayerXTurn = !isPlayerXTurn;
        statusImage.sprite = isPlayerXTurn ? xTurnSprite : oTurnSprite;

        CheckForAITurn();
    }

    bool CheckWin(string player)
    {
        // All possible winning combinations (Indices)
        int[,] lines = new int[,] {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Rows
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Cols
            {0, 4, 8}, {2, 4, 6}             // Diags
        };

        for (int i = 0; i < 8; i++)
        {
            if (boardState[lines[i, 0]] == player &&
                boardState[lines[i, 1]] == player &&
                boardState[lines[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    void HighlightWinLine(string player)
    {
        int[,] lines = new int[,] {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        for (int i = 0; i < 8; i++)
        {
            // If this is the winning line...
            if (boardState[lines[i, 0]] == player &&
                boardState[lines[i, 1]] == player &&
                boardState[lines[i, 2]] == player)
            {
                // ...Turn on the visuals
                // Using GetChild(0)
                gridButtons[lines[i, 0]].transform.GetChild(0).gameObject.SetActive(true);
                gridButtons[lines[i, 1]].transform.GetChild(0).gameObject.SetActive(true);
                gridButtons[lines[i, 2]].transform.GetChild(0).gameObject.SetActive(true);
                break;
            }
        }
    }

    public void OnBackButtonClicked()
    {
        CancelInvoke(); // Stop the AI if it is mid-calculation
        gameActive = false;

        gameCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void ResetGame()
    {
        CancelInvoke();

        gameActive = true;
        isPlayerXTurn = true;
        moveCount = 0;
        statusImage.sprite = xTurnSprite;

        // Clear data and UI
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = "";

            // Clear the child image
            Image btnImage = gridButtons[i].transform.GetChild(1).GetComponent<Image>();
            btnImage.sprite = null;
            btnImage.color = new Color(1, 1, 1, 0); // Invisible

            // Hide the win highlight
            gridButtons[i].transform.GetChild(0).gameObject.SetActive(false);

            // Re-enable the buttons
            gridButtons[i].GetComponent<Button>().interactable = true;
        }

        CheckForAITurn();
    }


    // --- AI LOGIC ---

    void CheckForAITurn()
    {
        if (!gameActive) return;

        PlayerType nextType = isPlayerXTurn ? playerXType : playerOType;

        if (nextType == PlayerType.AI)
        {
            // Add a small delay so we can see the moves happen
            Invoke("ExecuteAI", 0.5f);
        }
    }

    void ExecuteAI()
    {
        // 1. Determine symbols
        string aiSymbol = isPlayerXTurn ? "X" : "O";
        string humanSymbol = isPlayerXTurn ? "O" : "X"; // (Opponent symbol)

        // 2. Ask the AI script for the move
        int bestMove = TicTacToeAI.GetBestMove(boardState, aiSymbol, humanSymbol);

        // 3. Execute
        if (bestMove != -1)
        {
            OnCellClicked(bestMove, true);
        }
    }
}