using PurrNet;
using UnityEngine;

public class Tag_Game : NetworkIdentity
{
    //private SyncVar<>
    public bool isIt = false;

    [SerializeField]private Renderer _playerRenderer;

    void Start()
    {
        UpdateColor(isIt);
    }
    private void UpdateColor(bool it)
    {
        
    }
    protected override void OnSpawned()
    {
        base.OnSpawned();
        //if () { }
    }
}
