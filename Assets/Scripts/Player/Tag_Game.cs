using PurrNet;
using UnityEngine;

public class Tag_Game : NetworkIdentity
{
    public readonly SyncVar<bool> isIt = new SyncVar<bool>(false);

    [SerializeField] private Renderer _playerRenderer;
    [SerializeField] private Color _taggerColor = Color.red;
    [SerializeField] private Color _runnerColor = Color.white;
    [SerializeField] private float _tagCooldown = 2.0f;
    private float _nextTagTime = 0f;

    private void Awake()
    {
        isIt.onChanged += IsIt_onChanged;
    }

    protected override void OnDestroy()
    {
        isIt.onChanged -= IsIt_onChanged;
    }

    private void IsIt_onChanged(bool obj)
    {
        UpdateColor(obj);
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (isServer)
        {
            if (networkManager.playerCount <= 1)
            {
                isIt.value = true;
            }
        }

        UpdateColor(isIt.value);
    }

    private void UpdateColor(bool it)
    {
        _playerRenderer.material.color = it ? _taggerColor : _runnerColor;
    }

    [ServerRpc]
    public void PassTagServerRpc(Tag_Game targetPlayer)
    {
        if (!isIt.value) return;

        if (Time.time < _nextTagTime)
        {
            Debug.Log("Berek ma jeszcze blokadę ataku!");
            return;
        }
        isIt.value = false;
        targetPlayer.isIt.value = true;

        targetPlayer._nextTagTime = Time.time + _tagCooldown;

        Debug.Log($"Berek przekazany graczowi {targetPlayer.gameObject.name}. Blokada do: {targetPlayer._nextTagTime}");
    }
}
