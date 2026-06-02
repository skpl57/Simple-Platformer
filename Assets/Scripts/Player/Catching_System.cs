using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Catching_System : MonoBehaviour
{
    [SerializeField] private float _catchRange = 1.5f;
    [SerializeField] private RawImage _crosshair;
    [SerializeField] private Camera _camera;

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
        _crosshair.color = _colorFadedOut;
    }
    void Update()
    {
        bool inRange = Physics.Raycast(transform.position + Vector3.up * 0.5f, _camera.transform.rotation * Vector3.forward, out RaycastHit hit, _catchRange);
        if (inRange)
        {
            _crosshair.color = _colorFadedIn;
            if (Mouse.current.leftButton.isPressed)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player caught!");
                }
            }
        }
        else _crosshair.color = _colorFadedOut;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Vector3 startPosition = transform.position + Vector3.up * 0.5f;
        Vector3 lookDirection = _camera.transform.rotation * Vector3.forward;
        Gizmos.DrawLine(startPosition, startPosition + lookDirection * _catchRange);
    }
}
