using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerShooting))]
public class EnemyAI : MonoBehaviour, ITurnObserverAI
{
    private Player _player;
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;

    // public void NotifyNextTurn(int playerTurn)
    // {
    //     if(_player.PlayerNumber == playerTurn)
    //     {
    //         ShotDot();
    //     } else 
    //     {
    //         SetPosition();
    //     }
    // }

    public void Action(int playerTurn)
    {
        if(playerTurn == _player.PlayerNumber)
        {
            ShotDot();
        } else 
        {
            SetPosition();
        }
    }

    [SerializeField] private Vector2 lastShootedDot;

    void Start()
    {
        gameObject.tag = "PlayerAI";
        _player = GetComponent<Player>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerShooting = GetComponent<PlayerShooting>();
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<TurnManager>().AddObserverAI(this);
    }

#region ShootingAI

    private void ShotDot()
    {
        Debug.Log("AI Shooting");

        //Na starcie wyszukujemy obiekt gracza i pobieramy jego pozycje
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector2 playerPosition = playerObject.GetComponent<PlayerMovement>().CurrentPlayerPosition;
        Vector2 playerPreviousPosition = playerObject.GetComponent<PlayerMovement>().PreviousPlayerPosition;

        //Wyszukujemy wszyskie kropki które pozostały w grze
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        List<Dot> dots = new List<Dot>();
        List<Dot> destroyedDots = new List<Dot>(); 
        List<Dot> undestroyedDots = new List<Dot>();

        foreach (GameObject dotObject in dotObjects)
        {
            Dot currentDot = dotObject.GetComponent<Dot>();
            if(currentDot.PlayerNumber == 1)
            {
                dots.Add(currentDot);
                if(currentDot.IsDestroyed)
                {
                    //Zniszzczone kropki dodajemy do osobnej listy
                    destroyedDots.Add(currentDot);
                } else 
                {
                    undestroyedDots.Add(currentDot);
                }
            }
        }

        //Wyszukujemy wszystkie kropki dookoła gracza
        List<Dot> dotsAroundPlayer = new List<Dot>();
        foreach (Dot dot in dots)
        {
            bool isValid = false;
            if(playerPosition.x == dot.DotPosition.x && (playerPosition.y - dot.DotPosition.y == 1 || playerPosition.y - dot.DotPosition.y == -1))
            {
                isValid = true;
            } else if((playerPosition.x - dot.DotPosition.x == 1 || playerPosition.x - dot.DotPosition.x == -1) && (playerPosition.y - dot.DotPosition.y == 1 || playerPosition.y - dot.DotPosition.y == -1))
            {
                isValid = true;
            } else if((playerPosition.x - dot.DotPosition.x == 1 || playerPosition.x - dot.DotPosition.x == -1) && (playerPosition.y == dot.DotPosition.y))
            {
                isValid = true;
            }

            if(isValid && !dot.IsDestroyed)
            {
                dotsAroundPlayer.Add(dot);
            }
        }


        double percentOfUndestroyedDots = (undestroyedDots.Count * 100) / dots.Count;
        bool canPlayerEscape = playerObject.GetComponent<PlayerMovement>().CanPlayerEscape();

        if(!canPlayerEscape)
        {
            Debug.Log("Player has no escape options. Destroying player");
            DestroyPlayer(playerObject.GetComponent<Player>());
        }
        else if(dotsAroundPlayer.Count == 0)
        {
            Debug.Log("Dots around player are destroyed. Destroying player");
            DestroyPlayer(playerObject.GetComponent<Player>());
        }
        else if(percentOfUndestroyedDots <= 25.0)
        {
            Debug.Log("Most of dots are destroyed. Trying to shoot the player");
            TryToShootPlayer(undestroyedDots);
        } else if(playerPosition == new Vector2(2, 2))
        {
            Debug.Log("Player is on 2,2 position. Shooting around the player");
            ShootAroundThePlayer(dotsAroundPlayer);
        } else 
        {
            Debug.Log("Player is escaping, shooting in his direction");
            ShootInDirectionOfPlayerMovement(playerPosition, playerPreviousPosition, undestroyedDots);
        }

        //Może dodaj kolejne warunki 
    }

    private void TryToShootPlayer(List<Dot> undestroyedDots)
    {
        Debug.Log("Trying to shoot player");
        int targetDot = Random.Range(0, undestroyedDots.Count - 1);
        _playerShooting.ShotDot(undestroyedDots[targetDot]);
    }

    private void ShootAroundThePlayer(List<Dot> dotsAroundPlayer)
    {
        Debug.Log("Shooting around the player");

        //Na ten moment jeszcze nie wiem na ile to zadziała i czy wgl.
        int targetDot = Random.Range(0, dotsAroundPlayer.Count - 1);     
        _playerShooting.ShotDot(dotsAroundPlayer[targetDot]);
    }

    private void DestroyPlayer(Player player)
    {
        Debug.Log("Destroying player");

        //Tutaj dodaj kod odpowiedzialny za zniszczenie gracza
    }

    private void ShootInDirectionOfPlayerMovement(Vector2 currentPlayerPosition, Vector2 previousPlayerPosition, List<Dot> undestroyedDots)
    {
        Debug.Log("Trying to shoot in direction of player");
        Vector2 targetDotPosition = new Vector2(0, 0);

        if(currentPlayerPosition.x != previousPlayerPosition.x)
        {
            if(currentPlayerPosition.x > previousPlayerPosition.x)
            {
                //Strzel w currentPlayerPosition.x + 1 jeśli istnieje
                targetDotPosition = new Vector2(currentPlayerPosition.x + 1, currentPlayerPosition.y);
            } else if(currentPlayerPosition.x < previousPlayerPosition.x)
            {
                //Strzel w currentPlayerPosition.x + 1 jeśli istnieje
                targetDotPosition = new Vector2(currentPlayerPosition.x - 1, currentPlayerPosition.y);
            }
        } else if(currentPlayerPosition.y != previousPlayerPosition.y)
        {
            if(currentPlayerPosition.y > previousPlayerPosition.y)
            {
                //Strzel w currentPlayerPosition.y + 1 jeśli istnieje
                targetDotPosition = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y + 1);
            } else if(currentPlayerPosition.y < previousPlayerPosition.y)
            {
                //Strzel w currentPlayerPosition.y - 1 jeśli istnieje
                targetDotPosition = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y + 1);
            }
        } 

        if(DotExist(targetDotPosition, undestroyedDots) && targetDotPosition != new Vector2(0, 0))
        {
            Dot targetDot = GetDotByCoordinates(targetDotPosition, undestroyedDots);
            _playerShooting.ShotDot(targetDot);
        } else 
        {
            TryToShootPlayer(undestroyedDots);
        }
    }

    private bool DotExist(Vector2 dotPosition, List<Dot> dotList)
    {
        foreach (Dot dot in dotList)
        {
            if(dot.DotPosition == dotPosition)
            {
                return true;
            }
        }

        return false;
    }

    private Dot GetDotByCoordinates(Vector2 dotPosition, List<Dot> dotList)
    {
        foreach (Dot dot in dotList)
        {
            if(dotPosition == dot.DotPosition)
            {
                return dot;
            }
        }

        return null;
    }

#endregion

#region MovingAI
    private void SetPosition()
    {
        Debug.Log("AI Setting position");

        //Na starcie wyszukujemy obiekt gracza i pobieramy jego pozycje
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        bool playerShooted = playerObject.GetComponent<PlayerShooting>().ShootedInCurrentTurn;
        lastShootedDot = playerObject.GetComponent<PlayerShooting>().LastDestroyedDot;

        //Wyszukujemy wszyskie kropki które pozostały w grze
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");
        List<Dot> dots = new List<Dot>();
        List<Dot> destroyedDots = new List<Dot>(); 
        List<Dot> undestroyedDots = new List<Dot>();

        foreach (GameObject dotObject in dotObjects)
        {
            Dot currentDot = dotObject.GetComponent<Dot>();
            if(currentDot.PlayerNumber == 0)
            {
                dots.Add(currentDot);
                if(currentDot.IsDestroyed)
                {
                    //Zniszzczone kropki dodajemy do osobnej listy
                    destroyedDots.Add(currentDot);
                } else 
                {
                    undestroyedDots.Add(currentDot);
                }
            }
        }

        //Wyszukujemy wszystkie kropki dookoła gracza
        List<Dot> dotsAroundPlayer = new List<Dot>();
        foreach (Dot dot in dots)
        {
            bool isValid = false;
            if(_playerMovement.CurrentPlayerPosition.x == dot.DotPosition.x && (_playerMovement.CurrentPlayerPosition.y - dot.DotPosition.y == 1 || _playerMovement.CurrentPlayerPosition.y - dot.DotPosition.y == -1))
            {
                isValid = true;
            } else if((_playerMovement.CurrentPlayerPosition.x - dot.DotPosition.x == 1 || _playerMovement.CurrentPlayerPosition.x - dot.DotPosition.x == -1) && (_playerMovement.CurrentPlayerPosition.y - dot.DotPosition.y == 1 || _playerMovement.CurrentPlayerPosition.y - dot.DotPosition.y == -1))
            {
                isValid = true;
            } else if((_playerMovement.CurrentPlayerPosition.x - dot.DotPosition.x == 1 || _playerMovement.CurrentPlayerPosition.x - dot.DotPosition.x == -1) && (_playerMovement.CurrentPlayerPosition.y == dot.DotPosition.y))
            {
                isValid = true;
            }

            if(isValid && !dot.IsDestroyed)
            {
                dotsAroundPlayer.Add(dot);
            }
        }

        if(dotsAroundPlayer.Count > 0)
        {
            List<Dot> edgeDots = new List<Dot>();
            foreach (Dot dot in dots)
            {
                //Te sekcje domyślnie mają wartość 4, jednak jeżeli plansza będzie miała mieć inny rozmiar to będzie trzeba pobierać maksymalne współrzędne
                int maxX = 4;
                int maxY = 4;
                
                bool validDot = false;
                if(!dot.IsDestroyed)
                {
                    if(dot.PlayerNumber == 0)
                    {
                        if(dot.DotPosition.x == 0 && (dot.DotPosition.y >= 0 && dot.DotPosition.y <= maxY))
                        {
                            validDot = true;
                        } else if((dot.DotPosition.x >= 0 && dot.DotPosition.x <= maxY) && dot.DotPosition.y == 4)
                        {
                            validDot = true;
                        } else if(dot.DotPosition.x == 4 && (dot.DotPosition.y >= 0 && dot.DotPosition.y <= 4))
                        {
                            validDot = true;
                        } else if((dot.DotPosition.x >= 0 && dot.DotPosition.x <= 4) && dot.DotPosition.y == 0)
                        {
                            validDot = true;
                        }
                    }
                }

                if(validDot)
                {
                    edgeDots.Add(dot);
                }
            }

            
            //Decyzja dot. ruchu 
            bool inDanger;
            float distX = _playerMovement.CurrentPlayerPosition.x - lastShootedDot.x;
            float distY = _playerMovement.CurrentPlayerPosition.y - lastShootedDot.y;
            Vector2 distanceToLastShootedDot = new Vector2(_playerMovement.CurrentPlayerPosition.x - lastShootedDot.x, _playerMovement.CurrentPlayerPosition.y - lastShootedDot.y);

            Debug.Log(distX);
            Debug.Log(distY);
            Debug.Log(distanceToLastShootedDot);

            if(distanceToLastShootedDot.x <= 1 && distanceToLastShootedDot.x >= -1 || distanceToLastShootedDot.y <= 1 && distanceToLastShootedDot.y >= -1)
            {
                inDanger = true;
            } else 
            {
                inDanger = false;
            }

            if(inDanger)
            {
                Debug.Log("Escaping");
                MoveEscape(undestroyedDots, edgeDots, lastShootedDot);
            } else 
            {
                Debug.Log("Random movement");
                MoveToRandomPosition(undestroyedDots);
            }
        } else 
        {
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            gameController.SendMessage("EndGame", true);
            Destroy(gameObject);
        }
        }

        

    private void MoveEscape(List<Dot> undestroyedDots, List<Dot> edgeDots, Vector2 lastShootedDot)
    {
        bool moved = false;

        Debug.Log("AI Moving ESCAPE");

        List<Dot> notNearTheEdgeDots = new List<Dot>();
        List<Dot> nearTheEdgeDots = new List<Dot>();
        foreach (Dot dot in undestroyedDots)
        {
            if(edgeDots.Contains(dot))
            {
                nearTheEdgeDots.Add(dot);
            } else 
            {
                notNearTheEdgeDots.Add(dot);
            }
        }

        //Dot bestDotToEscape;
        if(notNearTheEdgeDots.Count == 0)
        {
            Debug.Log("AI Moving ESCAPE: Moving to edges");
            
            List<Dot> possibleDotsToEscape = new List<Dot>();
            foreach (Dot dot in nearTheEdgeDots)
            {
                if(dot.CanPlayerReach(_playerMovement))
                {
                    possibleDotsToEscape.Add(dot);
                }
            }

            if(possibleDotsToEscape.Count > 0)
            {
                int position = Random.Range(0, possibleDotsToEscape.Count - 1);
                _playerMovement.SetPosition(possibleDotsToEscape[position]);
            } else 
            {
                Debug.Log("There's no dot to escape");
            }
        } else 
        {
            Debug.Log("AI Moving ESCAPE: Moving not near the edges");
            List<Dot> possibleDotsToEscape = new List<Dot>();
            foreach (Dot dot in undestroyedDots)
            {
                if(dot.CanPlayerReach(_playerMovement))
                {
                    possibleDotsToEscape.Add(dot);
                }
            }

            if(possibleDotsToEscape.Count > 0)
            {
                Dot bestDotToEscape = null;
                Vector2 distanceToLastShootedDot = new Vector2(0, 0);

                foreach (Dot dot in possibleDotsToEscape)
                {
                    Vector2 tempDistanceToLastShootedDot = new Vector2(dot.DotPosition.x - lastShootedDot.x, dot.DotPosition.y - lastShootedDot.y);   

                    //Ten warunek jest niepewny, może do wymiany
                    if(tempDistanceToLastShootedDot.x > distanceToLastShootedDot.x || tempDistanceToLastShootedDot.y > distanceToLastShootedDot.y)
                    {
                        distanceToLastShootedDot = tempDistanceToLastShootedDot;
                        bestDotToEscape = dot;
                    }       
                }

                if(bestDotToEscape != null)
                {
                    _playerMovement.SetPosition(bestDotToEscape);
                }
            }
        }
    }

    private void MoveToRandomPosition(List<Dot> undestroyedDots)
    {
        Debug.Log("AI Moving Random");

        bool possibleMove = false;
        foreach(Dot dot in undestroyedDots)
        {
            if(dot.CanPlayerReach(_playerMovement))
            {
                possibleMove = true;
                break;
            }
        }

        if(possibleMove)
        {
            if(undestroyedDots.Count > 0)
            {
                Dot dot;
                do
                {
                    int randomPosition = Random.Range(0, undestroyedDots.Count - 1);
                    dot = undestroyedDots[randomPosition];
                } while(!dot.CanPlayerReach(_playerMovement));
                
                Debug.Log($"Random position setted to {dot.DotPosition}", dot);
                _playerMovement.SetPosition(dot);
            } 
        } else 
        {
            GameObject gameManager = GameObject.Find("GameController");
            gameManager.SendMessage("EndGame", true);
        }
    }

    #endregion
}
