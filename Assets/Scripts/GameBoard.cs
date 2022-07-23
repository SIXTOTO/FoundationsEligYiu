using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameResult{PlayerWin, AiWin, Tie, Unknown}
public class PiecePosition
{
    public int MyCoordX;
    public int MyCoordY;

    public PiecePosition(int myCoordX, int myCoordY)
    {
        MyCoordX = myCoordX;
        MyCoordY = myCoordY;
    }
}

public class BoardPlace
{
    private bool _isPlayerOccupied;
    private bool _isAiOccupied;

    public bool IsOccupied()
    {
        return _isPlayerOccupied || _isAiOccupied;
    }

    public bool IsPlayerOccupied()
    {
        return _isPlayerOccupied;
    }

    public void SetPlayerOccupied(bool isPlayerOccupied)
    {
        _isPlayerOccupied = isPlayerOccupied;
    }

    public bool IsAiOccupied()
    {
        return _isAiOccupied;
    }

    public void SetAiOccupied(bool isAiOccupied)
    {
        _isAiOccupied = isAiOccupied;
    }
}

public class GameBoard : MonoBehaviour
{
    private const int XSize = 3;
    private const int YSize = 3;
    private int _totalNumberOfEmptyPlace = XSize * YSize;

    private BoardPlace[,] boardPlaces = new BoardPlace[XSize, YSize];

    private void Awake()
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < XSize; y++)
            {
                boardPlaces[x, y] = new BoardPlace();
            }            
        }
    }
    
    public void SetBoardPlacePlayerOccupied(int coordX, int coordY)
    {
        boardPlaces[coordX, coordY].SetPlayerOccupied(true);
        _totalNumberOfEmptyPlace--;
    }

    public void SetBoardPlaceAiOccupied(int coordX, int coordY)
    {
        boardPlaces[coordX, coordY].SetAiOccupied(true);
        _totalNumberOfEmptyPlace--;
    }

    public void ClearBoard()
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                boardPlaces[x, y].SetPlayerOccupied(false);
                boardPlaces[x, y].SetAiOccupied(false);
            }
        }
        _totalNumberOfEmptyPlace = XSize * YSize; 
    }

    public PiecePosition[] GetAllEmptyPlaces()
    {
        var allEmptyPlaces = new List<PiecePosition>();
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (!IsBoardPlaceOccupied(x, y))
                {
                    allEmptyPlaces.Add(new PiecePosition(x, y));
                }
            }
        }
        return (PiecePosition[])(allEmptyPlaces.ToArray());
    }

    public bool IsBoardPlacePlayerOccupied(int coordX, int coordY)
    {
        return boardPlaces[coordX, coordY].IsPlayerOccupied();
    }

    public bool IsBoardPlaceAiOccupied(int coordX, int coordY)
    {
        return boardPlaces[coordX, coordY].IsAiOccupied();
    }

    public bool IsBoardPlaceOccupied(int coordX, int coordY)
    {
        return boardPlaces[coordX, coordY].IsOccupied();
    }
    

    
    public PiecePosition CalculateAiMove(int aiLevel)
    {
        //Find any position that can immediately win the game
        PiecePosition immediateWinPosition = FindImmediateWin();
        if (immediateWinPosition != null)
        {
            return immediateWinPosition;
        }
        
        //No matter what ailevel is chosen, we need to find any position that need to be immediately blocked to stop player from winning
        PiecePosition immediateBlockPosition = FindImmediateBlock();
        if (immediateBlockPosition != null)
        {
            return immediateBlockPosition;
        }
        
        PiecePosition[] allEmptyPlaces = GetAllEmptyPlaces();
        if (aiLevel > 0)
        {
            //Hard Level Strategy :
            //Find vacant position that can potentially win the game
            PiecePosition potentialWinPosition = FindPotentialWin();
            if (potentialWinPosition != null)
            {
                return potentialWinPosition;
            }
        }
        //Easy Level Strategy :
        //Play reactively (passively). Just randomly select an empty position
        return allEmptyPlaces.Length>0 ? allEmptyPlaces[Random.Range(0, allEmptyPlaces.Length)] : null;
    }

    public GameResult CheckGameResult(Player player)
    {
        // There are only 8 ways to win the game
        if (player == Player.Player)
        {
           return (IsBoardPlacePlayerOccupied(0,0) && IsBoardPlacePlayerOccupied(1,0) && IsBoardPlacePlayerOccupied(2,0)) ||
                  (IsBoardPlacePlayerOccupied(0,1) && IsBoardPlacePlayerOccupied(1,1) && IsBoardPlacePlayerOccupied(2,1)) ||
                  (IsBoardPlacePlayerOccupied(0,2) && IsBoardPlacePlayerOccupied(1,2) && IsBoardPlacePlayerOccupied(2,2)) ||
                  (IsBoardPlacePlayerOccupied(0,0) && IsBoardPlacePlayerOccupied(0,1) && IsBoardPlacePlayerOccupied(0,2)) ||
                  (IsBoardPlacePlayerOccupied(1,0) && IsBoardPlacePlayerOccupied(1,1) && IsBoardPlacePlayerOccupied(1,2)) ||
                  (IsBoardPlacePlayerOccupied(2,0) && IsBoardPlacePlayerOccupied(2,1) && IsBoardPlacePlayerOccupied(2,2)) ||
                  (IsBoardPlacePlayerOccupied(0,0) && IsBoardPlacePlayerOccupied(1,1) && IsBoardPlacePlayerOccupied(2,2)) ||
                  (IsBoardPlacePlayerOccupied(2,0) && IsBoardPlacePlayerOccupied(1,1) && IsBoardPlacePlayerOccupied(0,2)) ? GameResult.PlayerWin : (_totalNumberOfEmptyPlace < 1 ? GameResult.Tie : GameResult.Unknown);
        }else
        {
            return (IsBoardPlaceAiOccupied(0,0) && IsBoardPlaceAiOccupied(1,0) && IsBoardPlaceAiOccupied(2,0)) ||
                   (IsBoardPlaceAiOccupied(0,1) && IsBoardPlaceAiOccupied(1,1) && IsBoardPlaceAiOccupied(2,1)) ||
                   (IsBoardPlaceAiOccupied(0,2) && IsBoardPlaceAiOccupied(1,2) && IsBoardPlaceAiOccupied(2,2)) ||
                   (IsBoardPlaceAiOccupied(0,0) && IsBoardPlaceAiOccupied(0,1) && IsBoardPlaceAiOccupied(0,2)) ||
                   (IsBoardPlaceAiOccupied(1,0) && IsBoardPlaceAiOccupied(1,1) && IsBoardPlaceAiOccupied(1,2)) ||
                   (IsBoardPlaceAiOccupied(2,0) && IsBoardPlaceAiOccupied(2,1) && IsBoardPlaceAiOccupied(2,2)) ||
                   (IsBoardPlaceAiOccupied(0,0) && IsBoardPlaceAiOccupied(1,1) && IsBoardPlaceAiOccupied(2,2)) ||
                   (IsBoardPlaceAiOccupied(2,0) && IsBoardPlaceAiOccupied(1,1) && IsBoardPlaceAiOccupied(0,2)) ? GameResult.AiWin : (_totalNumberOfEmptyPlace < 1 ? GameResult.Tie : GameResult.Unknown);
        }
    }
    
    public PiecePosition FindImmediateBlock()
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (!IsBoardPlaceOccupied(x, y))
                {
                    //Pretend player put piece in this vacant position and check result
                    boardPlaces[x, y].SetPlayerOccupied(true);
                    if (CheckGameResult(Player.Player) == GameResult.PlayerWin)
                    {
                        boardPlaces[x, y].SetPlayerOccupied(false);
                        return new PiecePosition(x, y);
                    }
                    boardPlaces[x, y].SetPlayerOccupied(false);
                }
            }
        }
        return null;
    }
    
    public PiecePosition FindImmediateWin()
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (!IsBoardPlaceOccupied(x, y))
                {
                    //Pretend AI put piece in this vacant position and check result
                    boardPlaces[x, y].SetAiOccupied(true);
                    if (CheckGameResult(Player.Ai) == GameResult.AiWin)
                    {
                        boardPlaces[x, y].SetAiOccupied(false);
                        return new PiecePosition(x, y);
                    }
                    boardPlaces[x, y].SetAiOccupied(false);
                }
            }
        }
        return null;
    }
    public PiecePosition FindPotentialWin()
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (!IsBoardPlaceOccupied(x, y))
                {
                    //Pretend AI put piece in this vacant position and check potential win
                    boardPlaces[x, y].SetAiOccupied(true);
                    
                    if (FindImmediateWin() != null)
                    {
                        boardPlaces[x, y].SetAiOccupied(false);
                        return new PiecePosition(x, y);
                    }
                    boardPlaces[x, y].SetAiOccupied(false);
                }
            }
        }
        return null;
    }
    
}


