using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Sprites")]
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _normalDotSprite;
    [SerializeField] private Sprite _destroyedDotSprite;

    [SerializeField] private Vector2 _dotPosition;
    public Vector2 DotPosition{
        get{
            return _dotPosition;
        }
        set{
            _dotPosition = value;
        }
    }

    private bool _isDestroyed;
    public bool IsDestroyed
    {
        get
        {
            return _isDestroyed;
        } 
        private set
        {
            _isDestroyed = value;
        }
    }

    private int _playerNumber;
    public int PlayerNumber 
    {
        get
        {
            return _playerNumber;
        } 
        set 
        {
            _playerNumber = value;
        }
    }

    public bool CanPlayerReach(PlayerMovement player)
    {
        Vector2 playerPosition = player.CurrentPlayerPosition;

        if(!IsDestroyed) //Sprawdzenie czy kropka do której gracz chce się przemieścić nie jest zniszczona
        {
            if(DotPosition.x - playerPosition.x > 1 || DotPosition.y - playerPosition.y > 1) //Sprawdzenie czy dystans do kropki jest odpowiedni
            {
                return false;
            } else 
            {
                //Sprawdzenie czy nie jest to próba wykonania ruchu "na skos" 
                if((DotPosition.x > playerPosition.x || DotPosition.x < playerPosition.x) && DotPosition.y == playerPosition.y)
                {
                    return true;
                } else if(DotPosition.x == playerPosition.x && (DotPosition.y > playerPosition.y || DotPosition.y < playerPosition.y))
                {
                    return true;
                } else 
                {
                    return false;
                }
            }
        } else 
        {
            return false;
        }

        // if(DotPosition.x - player.CurrentPlayerPosition.x >= 2 && DotPosition.y - player.CurrentPlayerPosition.y >= 2)
        // {
        //     return false;
        // } else 
        // {
        //     if(!IsDestroyed)
        //     {
        //         return true;
        //     } else 
        //     {
        //         return false;
        //     }
        // }
    }

    public void CreateDot(Vector2 position, int playerNumber)
    {
        PlayerNumber = playerNumber;
        DotPosition = position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _normalDotSprite;
    }

    public void DestroyDot()
    {
        IsDestroyed = true;
        _spriteRenderer.sprite = _destroyedDotSprite;
    }
}
