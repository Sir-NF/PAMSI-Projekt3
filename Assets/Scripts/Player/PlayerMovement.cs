using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerShooting))]
public class PlayerMovement : MonoBehaviour
{
    private Player _player;
    public int PlayerNumber
    {
        get => _player.PlayerNumber;
    }

    private int _playerDotsNumber;

    [SerializeField] private int _movesPerTurn;
    [SerializeField] private int _movesInCurrentTurn; 
    public int MovesInCurrentTurn
    {
        get => _movesInCurrentTurn;
        set 
        {
            _movesInCurrentTurn = value;
            foreach (IPlayerActionsObserver observer in _playerActionsObservers)
            {
                observer.UpdateActions(_movesInCurrentTurn);
            }
        }
    }

    [SerializeField] private Vector2 currentPlayerPosition;
    public Vector2 CurrentPlayerPosition
    {
        get
        {
            return currentPlayerPosition;
        }
        protected set
        {
            currentPlayerPosition = value;
        }
    }
    [SerializeField] private Vector2 _previousPlayerPosition;
    public Vector2 PreviousPlayerPosition
    {
        get => _previousPlayerPosition;
        private set => _previousPlayerPosition = value;
    }

    [SerializeField] protected bool _movedInCurrentTurn;
    public bool MovedInCurrentTurn
    {
        get => _movedInCurrentTurn;
        set => _movedInCurrentTurn = value;
    }

    [SerializeField] protected bool _moveInCurrentTurn;
    public bool MoveInCurrentTurn
    {
        get => _moveInCurrentTurn;
        set 
        {
            if(value == true)
            {
                MovesInCurrentTurn = _movesPerTurn;
            }
            _moveInCurrentTurn = value;
        }
    }

    private List<IPlayerActionsObserver> _playerActionsObservers = new List<IPlayerActionsObserver>();
    private void Start() 
    {
        _player = GetComponent<Player>();
        _movesPerTurn = 2;
        
        if(PlayerNumber == 1)
        {
            _playerDotsNumber = 1;
        } else 
        {
            _playerDotsNumber = 0;
        }
    }

    public void AddActionObserver(IPlayerActionsObserver observer)
    {
        _playerActionsObservers.Add(observer);
    }

    public void SetPosition(Dot targetDot)
    {
        if(_moveInCurrentTurn)
        {
            if(targetDot.CanPlayerReach(this) && _movesInCurrentTurn > 0 && targetDot.PlayerNumber == _playerDotsNumber)
            {
                gameObject.transform.SetParent(targetDot.gameObject.transform);
                gameObject.transform.position = targetDot.transform.position;
                PreviousPlayerPosition = CurrentPlayerPosition;
                CurrentPlayerPosition = new Vector2(targetDot.DotPosition.x, targetDot.DotPosition.y);
                Debug.Log($"Setted new position at {targetDot.DotPosition}", this);
                
                foreach(IPlayerObserver observer in _player.PlayerObservers)
                {
                    observer.PlayerSettedPosition(this);
                }
                
                MovesInCurrentTurn--;
                MovedInCurrentTurn = true;
            }
        } else {
            Debug.Log("It's not your turn.");
        }
    }

    public void SetStartPosition(Dot targetDot)
    {
        gameObject.transform.SetParent(targetDot.gameObject.transform);
        gameObject.transform.position = targetDot.transform.position;
        CurrentPlayerPosition = new Vector2(targetDot.DotPosition.x, targetDot.DotPosition.y);
        Debug.Log($"Setted position at {targetDot.DotPosition}");
    }

    public bool CanPlayerEscape()
    {
        GameObject[] dots = GameObject.FindGameObjectsWithTag("Dot");

        foreach (GameObject dotObject in dots)
        {
            Dot dot = dotObject.GetComponent<Dot>();
            if(dot.PlayerNumber == _playerDotsNumber && !dot.IsDestroyed && dot.CanPlayerReach(this))
            {
                return true;
            }
        }

        return false;
    }
}
