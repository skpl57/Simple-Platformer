using UnityEngine;
using UnityEngine.InputSystem;
using PurrNet;

public class Camera_Rotation : NetworkBehaviour
{
    [Header("Myszka")]
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private float _fovSpeed = 8f;
    [SerializeField] private float _sprintFov = 80f;
    [SerializeField] private Transform _playerBody;
    
    
    private Player_Movement _playerMovement;
    private Camera _mainCamera;
    private float _xRotation = 0f;

    void Start()
    {
        _mainCamera = GetComponent<Camera>();
        _playerMovement = _playerBody.GetComponent<Player_Movement>();

        if (!isOwner)
        {
            _mainCamera.enabled = false;
            if (TryGetComponent<AudioListener>(out var listener))
            {
                listener.enabled = false;
            }
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!isOwner) return;

        HandleRotation();
        HandleFOV();
    }

    private void HandleRotation()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * _mouseSensitivity;
        float mouseY = mouseDelta.y * _mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        _playerBody.Rotate(Vector3.up * mouseX);
    }

    private void HandleFOV()
    {
        float currentModifier = _playerMovement.SavedVelocityModifier;
        float sprint = _playerMovement.SprintModifier;
        float crouch = _playerMovement.CrouchModifier;

        float targetFOV = _sprintFov - 10f;
        if (currentModifier == sprint) targetFOV = _sprintFov;
        else if (currentModifier == crouch) targetFOV = _sprintFov - 20f;

        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime * _fovSpeed);
    }
}
