using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IGameManager
{
    private GameObject _player;
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;

    public void StartGame() 
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _playerShooting = _player.GetComponent<PlayerShooting>();
    }

    void Update() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            MouseButtonPressed(0);
        } else if(Input.GetMouseButtonDown(1))
        {
            MouseButtonPressed(1);
        }
    }

    private void MouseButtonPressed(int mouseButton) 
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

        if (hit.collider != null) 
        {
            //Debug.Log($"Somethink clicked: {hit.collider.name}");
            if (hit.collider.tag == "Dot") 
            {
                Dot dot = hit.collider.gameObject.GetComponent<Dot>();
                
                if(mouseButton == 0)
                {
                    _playerMovement.SetPosition(dot);
                } else if(mouseButton == 1)
                {
                    _playerShooting.ShotDot(dot);
                }
            }
        }
    }
}