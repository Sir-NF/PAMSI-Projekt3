using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotManager))]
[RequireComponent(typeof(TurnManager))]
[RequireComponent(typeof(InputManager))]
public class GameController : MonoBehaviour, ITurnObserver
{
    private List<IGameManager> _gameManagers = new List<IGameManager>();
    [SerializeField] private GameObject _playerPrefab;

    private PlayerMovement _playerOneMovement;
    private PlayerMovement _playerTwoMovement;

    public void NotifyNextTurn(int playerTurn)
    {
        if(!_playerTwoMovement.CanPlayerEscape())
        {
            Win();
        }

        if(!_playerOneMovement.CanPlayerEscape())
        {
            Lose();
        }
    }

    private void Awake()
    {        
        GetComponents(_gameManagers);

        GameObject playerOne = Instantiate(_playerPrefab);
        GameObject playerTwo = Instantiate(_playerPrefab);

        playerOne.GetComponent<Player>().PlayerNumber = 1;
        playerTwo.GetComponent<Player>().PlayerNumber = 2;
        playerTwo.AddComponent(typeof(EnemyAI));

        foreach (IGameManager manager in _gameManagers)
        {
            manager.StartGame();
        }

        DotManager dotManager = GetComponent<DotManager>();

        playerOne.GetComponent<PlayerMovement>().SetStartPosition(dotManager.GetStartDot(1));
        playerTwo.GetComponent<PlayerMovement>().SetStartPosition(dotManager.GetStartDot(2));

        _playerOneMovement = GetComponent<PlayerMovement>();
        _playerTwoMovement = GetComponent<PlayerMovement>();

        GetComponent<TurnManager>().StartFirstTurn();
    }

    private void Win()
    {
        Debug.Log("You win");
    }

    private void Lose()
    {
        Debug.Log("You lose");
    }
}
