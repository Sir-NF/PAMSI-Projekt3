using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour, ITurnObserver, IGameManager, IPlayerActionsObserver, IEndGameObserver
{
    private TMP_Text _playerTurnInfoText;
    private TMP_Text _playerAvailableActionsText;
    private Button _finishTheTurnButton;
    private Button _debugFinishTheTurnButton;
    private GameObject _player;
    private TMP_Text _winLoseText;

    public void EndGame(bool playerWin)
    {
        Debug.Log("ENDGAME");
        if(playerWin)
        {
            _winLoseText.text  = "YOU WIN";
        } else 
        {
            _winLoseText.text = "YOU LOSE";
        }
    }

    public void NotifyNextTurn(int playerTurn)
    {
        if(playerTurn == 1)
        {
            _playerTurnInfoText.text = "Your turn to: Shoot";
            _playerAvailableActionsText.text = _player.GetComponent<PlayerShooting>().ShootsInCurrentTurn.ToString();
        } else 
        {
            _playerTurnInfoText.text = "Your turn to: Move";
            _playerAvailableActionsText.text = _player.GetComponent<PlayerMovement>().MovesInCurrentTurn.ToString();
        }
    }

    public void StartGame() 
    {
        GameObject playerTurnInfoObject = GameObject.Find("PlayerTurnInfoText");
        _playerTurnInfoText = playerTurnInfoObject.GetComponent<TMP_Text>();

        GameObject playerAvailableActionsObject = GameObject.Find("PlayerAvailableActionsText");
        _playerAvailableActionsText = playerAvailableActionsObject.GetComponent<TMP_Text>();

        GameObject finishTheTurnObject = GameObject.Find("TurnEndButton");
        _finishTheTurnButton = finishTheTurnObject.GetComponent<Button>();
        _finishTheTurnButton.onClick.AddListener(() => GetComponent<TurnManager>().EndCurrentTurn());

        GameObject debugFinishTheTurnObject = GameObject.Find("DEBUG_TurnEndButton");
        if(debugFinishTheTurnObject != null)
        {
            _debugFinishTheTurnButton = debugFinishTheTurnObject.GetComponent<Button>();
            _debugFinishTheTurnButton.onClick.AddListener(() => GetComponent<TurnManager>().DebugEndCurrentTurn());
        }

        GameObject endGameText = GameObject.Find("EndGameText");
        _winLoseText = endGameText.GetComponent<TMP_Text>();

        GetComponent<TurnManager>().AddObserver(this);
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.GetComponent<PlayerMovement>().AddActionObserver(this);
        _player.GetComponent<PlayerShooting>().AddActionObserver(this);
    }

    public void UpdateActions(int availableActions)
    {
        _playerAvailableActionsText.text = availableActions.ToString();
    }
}