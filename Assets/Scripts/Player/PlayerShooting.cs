using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerShooting : MonoBehaviour
{
    private Player _player;
    public int PlayerNumber
    {
        get => _player.PlayerNumber;
    }

    private PlayerMovement _playerMovement;

    [SerializeField] private bool _shootInCurrentTurn;
    public bool ShootInCurrentTurn
    {
        get => _shootInCurrentTurn;
        set 
        {
            if(value == true)
            {
                ShootsInCurrentTurn = 1;
            }
            _shootedInCurrentTurn = false;
            _shootInCurrentTurn = value;
        } 
    }

    [SerializeField] private bool _shootedInCurrentTurn;
    public bool ShootedInCurrentTurn
    {
        get => _shootedInCurrentTurn;
        set => _shootedInCurrentTurn = value;
    }

    [SerializeField] private int _shootsPerTurn;
    [SerializeField] private int _shootsInCurrentTurn;
    public int ShootsInCurrentTurn
    {
        get => _shootsInCurrentTurn;
        set 
        {
            _shootsInCurrentTurn = value;
            foreach (IPlayerActionsObserver observer in _playerActionsObservers)
            {
                observer.UpdateActions(_shootsInCurrentTurn);
            }
        }
    }
    
    [SerializeField] private Vector2 lastDestroyedDot;
    public Vector2 LastDestroyedDot
    {
        get => lastDestroyedDot;
        private set => lastDestroyedDot = value;
    }

    private List<IPlayerActionsObserver> _playerActionsObservers = new List<IPlayerActionsObserver>();

    private void Start() 
    {
        _shootsPerTurn = 1;
        _shootsInCurrentTurn = _shootsPerTurn;
        ShootedInCurrentTurn = false;
        _playerMovement = GetComponent<PlayerMovement>();
        _player = GetComponent<Player>();
    }

    public void AddActionObserver(IPlayerActionsObserver observer)
    {
        _playerActionsObservers.Add(observer);
    }

    public void ShotDot(Dot targetDot)
    {
        if(_shootInCurrentTurn && _shootsInCurrentTurn > 0)
        {
            if(!targetDot.IsDestroyed && targetDot.PlayerNumber != _player.PlayerNumber && !_shootedInCurrentTurn && targetDot.DotPosition != _playerMovement.CurrentPlayerPosition)
            {
                targetDot.DestroyDot();

                foreach(var observer in _player.PlayerObservers)
                {
                    observer.PlayerShooted(this); 
                }

                LastDestroyedDot = targetDot.DotPosition;
                ShootedInCurrentTurn = true;
                ShootsInCurrentTurn--;
            }
        } else 
        {
            Debug.Log("It's not your turn to shoot");
        }
    }    
}