using UnityEngine;
using System.Collections.Generic;

public static class TicTacToeAI
{

    public static int GetBestMove(string[] boardState, string aiSymbol, string humanSymbol)
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == "")
            {
                boardState[i] = aiSymbol;
                int score = Minimax(boardState, 0, false, aiSymbol, humanSymbol);
                boardState[i] = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }
        return bestMove;
    }

    private static int Minimax(string[] board, int depth, bool isMaximizing, string aiSym, string humSym)
    {
        if (CheckWin(board, aiSym)) return 10 - depth;
        if (CheckWin(board, humSym)) return depth - 10;
        if (IsBoardFull(board)) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = aiSym;
                    int score = Minimax(board, depth + 1, false, aiSym, humSym);
                    board[i] = "";
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = humSym;
                    int score = Minimax(board, depth + 1, true, aiSym, humSym);
                    board[i] = "";
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    // AI's internal helper to check wins on the virtual board
    private static bool CheckWin(string[] board, string player)
    {
        int[,] lines = new int[,] {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        for (int i = 0; i < 8; i++)
        {
            if (board[lines[i, 0]] == player &&
                board[lines[i, 1]] == player &&
                board[lines[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsBoardFull(string[] board)
    {
        foreach (string s in board) if (s == "") return false;
        return true;
    }
}