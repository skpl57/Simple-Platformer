using PurrNet;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Catching_System : NetworkIdentity
{
    [SerializeField] private float _catchRange = 1.5f;
    [SerializeField] private RawImage _crosshair;
    [SerializeField] private Camera _camera;

    private Hand_Animations _animations;
    private Tag_Game _tagGame;
    private Color _colorFadedOut;
    private Color _colorFadedIn;
    private void Start()
    {
        if (ColorUtility.TryParseHtmlString("#00000096", out Color fadeOut))
        {
            _colorFadedOut = fadeOut;
        }
        if (ColorUtility.TryParseHtmlString("#FFFFFFAF", out Color fadeIn))
        {
            _colorFadedIn = fadeIn;
        }
        _tagGame = GetComponent<Tag_Game>();
        _animations = GetComponent<Hand_Animations>();

        _crosshair.color = _colorFadedOut;
    }
    void Update()
    {
        if (!isOwner) return;

        bool inRange = Physics.Raycast(transform.position + Vector3.up * 0.5f, _camera.transform.rotation * Vector3.forward, out RaycastHit hit, _catchRange);
        //if (inRange && hit.collider.CompareTag("Player"))
        //{
        //    _animations.PointToPlayer = true;
        //}
        if (inRange && hit.collider.CompareTag("Player") && !hit.collider.GetComponent<NetworkIdentity>().isOwner)
        {
            _animations.PointToPlayer = true;
            _crosshair.color = _colorFadedIn;
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (hit.collider.TryGetComponent<Tag_Game>(out Tag_Game targetPlayer))
                {
                    if (_tagGame.isIt.value)
                    {
                        Debug.Log($"Passing tag to {targetPlayer.gameObject.name}");
                        _tagGame.PassTagServerRpc(targetPlayer);
                    }
                }
            }
        }
        else
        {
            _crosshair.color = _colorFadedOut;
            _animations.PointToPlayer = false;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Vector3 startPosition = transform.position + Vector3.up * 0.5f;
        Vector3 lookDirection = _camera.transform.rotation * Vector3.forward;
        Gizmos.DrawLine(startPosition, startPosition + lookDirection * _catchRange);
    }
}
