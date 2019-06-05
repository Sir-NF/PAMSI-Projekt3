using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerShooting))]
public class Player : MonoBehaviour, ITurnObserver
{
    [Header("Player parameters")]
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;

    [SerializeField] protected int _playerNumber;
    public int PlayerNumber
    {
        get
        {
            return _playerNumber;
        } 
        set 
        {
            SetPlayerSprite(value);
            _playerNumber = value;
        }
    }

    private List<IPlayerObserver> _playerObservers = new List<IPlayerObserver>();
    public List<IPlayerObserver> PlayerObservers 
    {
        get => _playerObservers;
    }
    

    [SerializeField] private Sprite _playerOneSprite;
    [SerializeField] private Sprite _playerTwoSprite;

    private void Awake() 
    {
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<TurnManager>().AddObserver(this);
        _playerMovement = GetComponent<PlayerMovement>();
        _playerShooting = GetComponent<PlayerShooting>();
    }

    private void SetPlayerSprite(int value)
    {
        if(value == 1)
        {
            GetComponent<SpriteRenderer>().sprite = _playerOneSprite;
        } else 
        {
            GetComponent<SpriteRenderer>().sprite = _playerTwoSprite;
        }
    }

    public void AddPlayerObserver(IPlayerObserver observer)
    {
        _playerObservers.Add(observer);
    }

    public void NotifyNextTurn(int playerTurn)
    {
        Debug.Log("NOTIFAJKA");
        if(playerTurn == _playerNumber)
        {
            _playerMovement.MoveInCurrentTurn = false;
            _playerShooting.ShootInCurrentTurn = true;
            Debug.Log($"Player{_playerNumber} strzela");
        } else 
        {
            _playerMovement.MoveInCurrentTurn = true;
            _playerShooting.ShootInCurrentTurn = false;
            Debug.Log($"Player{_playerNumber} idzie");
        }

        //Tutaj dodaj kod odpowiedzialny za powiadomienie UI o tym czy gracz ma strzelać czy ruszać się
    }
}
