using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum TicTacToeState{None, Cross, Circle}

public enum Player{Player, Ai}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{
	private int _aiLevel;
	private EndMessage endMessage;
	
    [SerializeField]
    private GameBoard gameBoard;

    [SerializeField]
    private GameObject gamePanel;
    
    [SerializeField]
	private bool isPlayerTurn;

	[SerializeField]
	private TicTacToeState playerState = TicTacToeState.Circle;
	TicTacToeState aiState = TicTacToeState.Cross;

	[FormerlySerializedAs("_xPrefab")] [SerializeField]
	private GameObject xPrefab;

	[FormerlySerializedAs("_oPrefab")] [SerializeField]
	private GameObject oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}

		gamePanel = GameObject.Find("GamePanel");
		endMessage = FindObjectOfType<EndMessage>();
		gamePanel.SetActive(false);
	}

	public void Start()
	{
		onPlayerWin.AddListener(endMessage.OnGameEnded);
	}

	public void StartAI(int aiLevel){
		_aiLevel = aiLevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, playerState);

		gameBoard.SetBoardPlacePlayerOccupied(coordX, coordY);

		//Check if player wins after the latest move
		var gameResult = gameBoard.CheckGameResult(Player.Player);
		if (gameResult == GameResult.Tie)
		{
			onPlayerWin.Invoke(-1);
		}else if (gameResult == GameResult.PlayerWin)
		{
			gamePanel.SetActive(true);
			onPlayerWin.Invoke(0);
		}
		else
		{

			PiecePosition nextAiMove = gameBoard.CalculateAiMove(_aiLevel);
			_triggers[nextAiMove.MyCoordX, nextAiMove.MyCoordY].SetInputEndabled(false);
			AiSelects(nextAiMove.MyCoordX, nextAiMove.MyCoordY);
		}
	}

	public void AiSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, aiState);
		
		gameBoard.SetBoardPlaceAiOccupied(coordX, coordY);

		//Check if AI wins after the latest move
		var gameResult = gameBoard.CheckGameResult(Player.Ai);
		if (gameResult == GameResult.Tie)
		{
			onPlayerWin.Invoke(-1);
		}else if (gameResult == GameResult.AiWin)
		{
			onPlayerWin.Invoke(1);
		}
		
        SetPlayerTurn(true);
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.Circle ? oPrefab : xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);

	}

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void SetPlayerTurn(bool playerTurn)
    {
        isPlayerTurn = playerTurn;
    }
}

