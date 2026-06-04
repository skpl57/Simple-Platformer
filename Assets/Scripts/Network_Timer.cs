using PurrNet;
using TMPro;
using UnityEngine;

public class Network_Timer : NetworkIdentity
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _endText;
    [SerializeField] private float _duration;
    private SyncTimer timer = new();
    private bool _gameStarted = false;
    private void Awake()
    {
        timer.onTimerSecondTick += OnTimerSecondTick;
        timer.onTimerEnd += OnTimerEnd;
    }
    protected override void OnSpawned(bool asServer)
    {
        _endText.gameObject.SetActive(false);
        OnTimerSecondTick();

        if (asServer)
        {
            networkManager.onPlayerJoined += OnPlayerJoined;
            CheckStartConditions();
        }
    }

    private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
    {
        CheckStartConditions();
    }

    private void CheckStartConditions()
    {
        if (_gameStarted) return;

        if (networkManager.playerCount >= 2)
        {
            _gameStarted = true;
            timer.StartTimer(60f * _duration);
            networkManager.onPlayerJoined -= OnPlayerJoined;
        }

    }
    private void OnTimerSecondTick()
    {
        string minutes = Mathf.Floor(timer.remainingInt / 60).ToString("0");
        string seconds = Mathf.Floor(timer.remainingInt % 60).ToString("00");
        _timerText.text = $"{minutes}:{seconds}";
    }
    private void OnTimerEnd()
    {
        _timerText.text = "0:00";
        if (isServer)
        {
            FindAndAnnounceLoser();
        }
    }
    private void FindAndAnnounceLoser()
    {
        Tag_Game[] allPlayers = FindObjectsByType(typeof(Tag_Game)) as Tag_Game[];
        PlayerID loserID = default;

        foreach (Tag_Game player in allPlayers)
        {
            if (player.isIt.value)
            {
                loserID = player.owner.Value;
                break;
            }
        }

        RpcShowEndGameMessage(loserID);
    }

    [ObserversRpc]
    private void RpcShowEndGameMessage(PlayerID loserID)
    {
        _endText.gameObject.SetActive(true);

        if (networkManager.localPlayer == loserID)
        {
            _endText.text = "Przegrana!";
            _endText.color = Color.red;
        }
        else
        {
            _endText.text = "Wygrana!";
            _endText.color = Color.green;
        }
    }
}