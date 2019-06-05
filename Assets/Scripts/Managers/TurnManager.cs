using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour, IGameManager, IPlayerObserver
{
    private List<ITurnObserver> _players = new List<ITurnObserver>();
    private List<ITurnObserverAI> _enemies = new List<ITurnObserverAI>();

    [Header("Turn parameters")]
    [SerializeField] private int _turnCounter = 1;

    [Range(1, 2)]
    [SerializeField] private int _playerTurn;
    [SerializeField] private bool _playerShooted;
    [SerializeField] private bool _playerSettedPosition;

    private EnemyAI enemy;

    public void StartGame() 
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<Player>().AddPlayerObserver(this);
        }
        _playerTurn = SetStartingPlayer();
    }

    public void StartFirstTurn()
    {
        _playerTurn = 1;
        NextTurnNotify(_playerTurn);
        Debug.Log("First turn started");
    }

    public void AddObserver(ITurnObserver observer)
    {
        _players.Add(observer);
    }

    public void AddObserverAI(ITurnObserverAI observer)
    {
        _enemies.Add(observer);
    }

    public void EndCurrentTurn()
    {
        if(_playerTurn == 1)
        {
            if(_playerShooted)
            {
                foreach (var enemy in _enemies)
                {
                    enemy.Action(_playerTurn);
                }
                _turnCounter++;
                _playerTurn++;
                NextTurnNotify(_playerTurn);
                _playerShooted = false;
                _playerSettedPosition = false;
            } else  
            {
                Debug.Log("Somethink is wrong. You have to shoot");
            }
        } else 
        {
            if(_playerSettedPosition)
            {
                //Wywołaj akcję AI
                foreach (var enemy in _enemies)
                {
                    enemy.Action(_playerTurn);
                }
                _turnCounter--;
                _playerTurn--;
                _playerSettedPosition = false;
                _playerShooted = false;

                NextTurnNotify(_playerTurn);
            } else 
            {
                Debug.Log("Somethink is wrong. You have to set new position");
            }
        }
    }

    public void DebugEndCurrentTurn()
    {
        _turnCounter++;

        if(_playerTurn == 1)
        {
            _playerTurn++;
        } else 
        {
            _playerTurn--;
        }

        _playerSettedPosition = false;
        _playerShooted = false;

        NextTurnNotify(_playerTurn);        
    }

    private void NextTurnNotify(int playerTurn)
    {
        foreach (ITurnObserver player in _players)
        {
            player.NotifyNextTurn(playerTurn);
        }
    }

    private int SetStartingPlayer()
    {
        //Tymczasowo nie będzie to losowo
        return 2;//Random.Range(1, 3);
    }

    public void PlayerSettedPosition(PlayerMovement player)
    {
        if(_playerTurn != player.PlayerNumber)
        {
            _playerSettedPosition = true;
        }
    }

    public void PlayerShooted(PlayerShooting player)
    {
        if(_playerTurn == player.PlayerNumber)
        {
            _playerShooted = true;
        }
    }
}