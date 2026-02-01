using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TicTacToeController : MonoBehaviour
{
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

        if (MainMenuController.isVsAI)
        {
            // Only lock the input if it is NOT coming from the AI
            if (!fromAI)
            {
                bool isMyTurn = (MainMenuController.playerSide == "X" && isPlayerXTurn) ||
                                (MainMenuController.playerSide == "O" && !isPlayerXTurn);

                if (!isMyTurn)
                {
                    // Deselect the button so it doesn't get stuck looking "clicked"
                    EventSystem.current.SetSelectedGameObject(null);
                    return; // Block the human
                }
            }
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
        //string nextPlayer = isPlayerXTurn ? "X" : "O";
        statusImage.sprite = isPlayerXTurn ? xTurnSprite : oTurnSprite;

        // 6. Trigger AI if needed
        if (MainMenuController.isVsAI && gameActive)
        {
            // AI plays if:
            // - Player chose X, but it's now O's turn
            // - Player chose O, but it's now X's turn
            bool isAITurn = (MainMenuController.playerSide == "X" && !isPlayerXTurn) ||
                            (MainMenuController.playerSide == "O" && isPlayerXTurn);

            if (isAITurn)
            {
                Invoke("PerformAIMove", 0.5f);
            }
        }
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

        // Check if AI needs to make the first move
        if (MainMenuController.isVsAI && MainMenuController.playerSide == "O")
        {
            // AI is X, so it goes first!
            Invoke("PerformAIMove", 0.5f);
        }
    }


    // --- AI LOGIC ---

    public void PerformAIMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        // 1. Determine who is who based on the player's choice
        string aiSymbol = (MainMenuController.playerSide == "X") ? "O" : "X";
        string humanSymbol = (MainMenuController.playerSide == "X") ? "X" : "O";

        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == "")
            {
                // 2. AI places ITS OWN symbol (not always "O")
                boardState[i] = aiSymbol;

                // 3. Run Minimax passing the correct symbols
                // We pass 'false' because the next turn is the Human's (Minimizer)
                int score = Minimax(boardState, 0, false, aiSymbol, humanSymbol);

                // Undo
                boardState[i] = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        if (bestMove != -1)
        {
            OnCellClicked(bestMove, true);
        }
    }

    // Now accepts aiSym and humSym to know who is winning/losing
    int Minimax(string[] board, int depth, bool isMaximizing, string aiSym, string humSym)
    {
        // Dynamic Check: Did the AI win? or the Human?
        if (CheckWin(aiSym)) return 10 - depth; // Good for AI
        if (CheckWin(humSym)) return depth - 10; // Bad for AI
        if (IsBoardFull()) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = aiSym; // AI's turn
                    int score = Minimax(board, depth + 1, false, aiSym, humSym);
                    board[i] = "";
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else // Minimizing (Human's turn)
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = humSym; // Human's turn
                    int score = Minimax(board, depth + 1, true, aiSym, humSym);
                    board[i] = "";
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    // Helper for the AI to check for draws
    bool IsBoardFull()
    {
        foreach (string s in boardState)
        {
            if (s == "") return false;
        }
        return true;
    }
}