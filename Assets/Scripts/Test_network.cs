using PurrNet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_network : NetworkIdentity
{
    //[SerializeField] private NetworkIdentity _networkIdentity;
    //[SerializeField] private Color _color;
    //[SerializeField] private Renderer _renderer;

    //[SerializeField] private SyncVar<int> health = new(100);

    //private void Update()
    //{
    //    if (Keyboard.current.pKey.isPressed)
    //    {
    //        SetColor();
    //    }
    //}

    //[ServerRpc] // dla każdego klienta, który wywoła tę metodę, zostanie ona wykonana na serwerze

    //[ObserversRpc (bufferLast: true)] // metoda zostanie wywołana na wszystkich klientach, którzy obserwują ten obiekt (w tym na serwerze), a jeśli bufferLast jest ustawione na true, to nowo dołączający klienci otrzymają ostatnie wywołanie tej metody
    //private void SetColor()
    //{
    //    _renderer.material.color = _color;
    //}   


}
