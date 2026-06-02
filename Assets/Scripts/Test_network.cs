using PurrNet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_network : NetworkIdentity
{
    [SerializeField] private NetworkIdentity _networkIdentity;
    [SerializeField] private Color _color;
    [SerializeField] private Renderer _renderer;

    [SerializeField] private SyncVar<int> health = new(100);

    private void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            SetColor();
        }
    }

    
    [ObserversRpc (bufferLast: true)]
    private void SetColor()
    {
        _renderer.material.color = _color;
    }   
}
