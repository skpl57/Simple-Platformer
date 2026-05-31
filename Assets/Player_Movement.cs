using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [Header("Ruch i Fizyka")]
    [SerializeField] private float _speedFloat = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _sprintModifier = 2f;
    [SerializeField] private float _crouchModifier = 0.5f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _downDrag = 0.5f;
    [SerializeField] private float _headHeight = 1.1f;

    [SerializeField] private float _mouseSensitivity = 0.1f;

    private Transform _cameraTransform;
    private float _wallCheckDistance = 1f;

    private CharacterController _controller;
    private float _xRotation = 0f;
    private float _savedVelocityModifier = 1f;

    private Vector3 _velocity;
    private Vector3 _movement = Vector3.zero;

    private Vector3 _boxPosition;
    private Vector3 _boxHalfExtents;
    private float _boxWidthModifier = 0.7f;
    private float _boxSide;
    private Camera _mainCamera;

    private bool _canJump = true;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = GetComponentInChildren<Camera>();
        _cameraTransform = _mainCamera.transform;
        Cursor.lockState = CursorLockMode.Locked;

        _boxSide = _controller.radius * _boxWidthModifier;
        _boxHalfExtents = new Vector3(_boxSide, 0.02f, _boxSide);
    }

    void Update()
    {
        _boxPosition = transform.position + new Vector3(0, _headHeight * transform.localScale.y, 0);
        bool isCeilingAbove = Physics.CheckBox(_boxPosition, _boxHalfExtents, transform.rotation, LayerMask.GetMask("Default"));
        _canJump = _controller.isGrounded;
        CalculateWallClimbing();

        if (!isCeilingAbove)
        {
            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                if (transform.localScale.y > 0.5f) _controller.Move(Vector3.down * _downDrag);
                transform.localScale = new Vector3(1f, 0.5f, 1f);
                _jumpHeight = 1f;
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                _jumpHeight = 2f;
            }
            if (_controller.isGrounded) _savedVelocityModifier = 1f;
            if (Keyboard.current.spaceKey.isPressed && _canJump) _velocity.y = Mathf.Sqrt(-_jumpHeight * _gravity * 1.5f); // Skok
        }

        CalculateBasicMovement();
        CameraMovement();

        Vector3 moveDirection = transform.TransformDirection(_movement);
        _controller.Move(moveDirection * _speedFloat * Time.deltaTime);

        if (_controller.isGrounded) _velocity.y = -2f;
        else if (!_canJump) _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        if (isCeilingAbove && _velocity.y > 0) _velocity.y = 0f;
    }

    private void CameraMovement()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * _mouseSensitivity;
        float mouseY = mouseDelta.y * _mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float fovSpeed = 8f;
        float targetFOV = (_savedVelocityModifier == _sprintModifier) ? 70f : (_savedVelocityModifier == _crouchModifier) ? 50f : 60f;
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }
    private void CalculateBasicMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ = 1f;
        if (Keyboard.current.sKey.isPressed) moveZ = -1f;
        if (Keyboard.current.dKey.isPressed) moveX = 1f;
        if (Keyboard.current.aKey.isPressed) moveX = -1f;

        if (moveZ != 1f && !_controller.isGrounded) _savedVelocityModifier = _crouchModifier;

        if (Keyboard.current.leftShiftKey.isPressed && _controller.isGrounded) _savedVelocityModifier = _sprintModifier;
        if (Keyboard.current.leftCtrlKey.isPressed && _controller.isGrounded) _savedVelocityModifier = _crouchModifier;

        _movement = new Vector3(moveX, 0f, moveZ).normalized;
        _movement *= _savedVelocityModifier;
    }

    private void CalculateWallClimbing()
    {
        Vector3 rayModifier = transform.forward * _wallCheckDistance * transform.localScale.y;
        bool canWallClimb = Physics.Raycast(transform.position + Vector3.up / 2f, transform.forward, _wallCheckDistance * transform.localScale.y, LayerMask.GetMask("Default")) &&
                             !Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, _wallCheckDistance * transform.localScale.y, LayerMask.GetMask("Default"));
        if (canWallClimb)
        {
            _velocity.y = 0f;
            _canJump = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up / 2f, transform.position + (Vector3.up / 2f) + transform.forward * _wallCheckDistance * transform.localScale.y);
        Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + Vector3.up * 1.5f + transform.forward * _wallCheckDistance * transform.localScale.y);

        if (Physics.CheckBox(_boxPosition, _boxHalfExtents, transform.rotation, LayerMask.GetMask("Default")))
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(_boxPosition, transform.rotation, transform.localScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, _boxHalfExtents);
        Gizmos.matrix = Matrix4x4.identity;
    }
}