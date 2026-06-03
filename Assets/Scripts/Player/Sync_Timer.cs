using PurrNet;
using TMPro;
using UnityEngine;

public class Sync_Timer : NetworkIdentity
{
    [SerializeField] private TMP_Text _timerText;
    private SyncTimer timer = new();
    private void Awake()
    {
        timer.onTimerSecondTick += OnTimerSecondTick;
    }
    protected override void OnSpawned(bool asServer)
    {
        timer.StartTimer(60f * 5);
    }

    private void OnTimerSecondTick()
    {

        string minutes = Mathf.Floor(timer.remainingInt / 60).ToString("");
        string seconds = Mathf.Floor(timer.remainingInt % 60).ToString("00");
        _timerText.text = $"{minutes}:{seconds}";
    }
}
