using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotManager : MonoBehaviour, IGameManager
{
    [Header("Generation parameters")]
    [SerializeField] private int verticalDotsAmount;
    [SerializeField] private int horizontalDotsAmount;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform playerSpawnPosition;
    [SerializeField] private Transform enemySpawnPosition;
    [SerializeField] private Transform  spawnPosition;
    [SerializeField] private Vector3 spaceBetweenDots;

    [SerializeField] private List<Dot> _playerOneDotsList = new List<Dot>();
    public List<Dot> PlayerOneDotsList
    {
        get 
        {
            return _playerOneDotsList;
        } 
    }

    [SerializeField] private List<Dot> _playerTwoDotsList = new List<Dot>();
    public List<Dot> PlayerTwoDotsList
    {
        get 
        {
            return _playerTwoDotsList;
        } 
    }

    public void StartGame()
    {
        GenerateDots();
    }

    public Dot GetStartDot(int playerNumber)
    {
        if(playerNumber == 1)
        {
            return GetDotByCoordinates(new Vector2(2, 2), PlayerOneDotsList);
        } else 
        {
            return GetDotByCoordinates(new Vector2(2, 2), PlayerTwoDotsList);
        }
    }
    
    private Dot GetDotByCoordinates(Vector2 dotPosition, List<Dot> dots)
    {
        foreach (Dot dot in dots)
        {
            if(dotPosition == dot.DotPosition)
            {
                return dot;
            }
        }

        return null;
    }

    private void GenerateDots()
    {
        for(int x = 0; x < 2; x++)
        {
            for (int i = 0; i < horizontalDotsAmount; i++)
            {
                if(x == 0)
                {
                    spawnPosition.transform.position = playerSpawnPosition.transform.position;
                } else
                {
                    spawnPosition.transform.position = enemySpawnPosition.transform.position;
                }

                if (i != 0)
                {
                    spawnPosition.transform.position = new Vector3(spawnPosition.position.x + (i * spaceBetweenDots.x), spawnPosition.transform.position.y, spawnPosition.transform.position.y);
                }

                GameObject dotHorizontal = Instantiate(dotPrefab, spawnPosition.transform, true);
                dotHorizontal.name = $"dotHorizontal{i}";
                dotHorizontal.GetComponent<Dot>().CreateDot(new Vector2(i, 0), x);

                if(x == 1)
                {
                    _playerOneDotsList.Add(dotHorizontal.GetComponent<Dot>());
                } else 
                {
                    _playerTwoDotsList.Add(dotHorizontal.GetComponent<Dot>());
                }

                for (int j = 1; j < verticalDotsAmount; j++)
                {
                    spawnPosition.transform.position = new Vector3(spawnPosition.position.x, spawnPosition.transform.position.y + spaceBetweenDots.y, spawnPosition.transform.position.z);

                    GameObject dotVertical = Instantiate(dotPrefab, spawnPosition.transform, true);
                    dotVertical.name = $"dotVertical{i}{j}";
                    dotVertical.GetComponent<Dot>().CreateDot(new Vector2(i, j), x);

                    if(x == 1)
                    {
                        PlayerOneDotsList.Add(dotVertical.GetComponent<Dot>());
                    } else 
                    {
                        PlayerTwoDotsList.Add(dotVertical.GetComponent<Dot>());
                    }
                }
            }
        }
    }
}
