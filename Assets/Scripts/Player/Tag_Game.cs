using PurrNet;
using UnityEngine;

public class Tag_Game : NetworkIdentity
{
    //private SyncVar<>
    public bool isIt = false;

    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        UpdateColor(isIt);
    }
    private void UpdateColor(bool it)
    {
        
    }
    protected override void OnSpawned()
    {
        base.OnSpawned();
    }
}
