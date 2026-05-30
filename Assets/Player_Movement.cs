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
    
    private float _raycastLenght = 1.1f;

    [SerializeField] private float _mouseSensitivity = 0.1f;
    private Transform _cameraTransform;

    private CharacterController _controller;
    private float _xRotation = 0f;
    private float _savedVelocityModifier = 1f;
    private Vector3 _velocity;
    private Vector3 _movement = Vector3.zero;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        Camera mainCamera = GetComponentInChildren<Camera>();
        _cameraTransform = mainCamera.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!Physics.Raycast(transform.position, Vector3.up, _raycastLenght, LayerMask.GetMask("Default")))
        {
            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                transform.localScale = new Vector3(1f, 0.5f, 1f);
            } // Przytrzymanie Ctrl zmniejsza postać
            else transform.localScale = new Vector3(1f, 1f, 1f);
            if (_controller.isGrounded) _savedVelocityModifier = 1f;
            if (Keyboard.current.spaceKey.isPressed && _controller.isGrounded) _velocity.y = Mathf.Sqrt(-_jumpHeight * _gravity * 1.5f); // Skok
        }
        //else _velocity.y = (0 - _velocity.y) / 2f;

        Debug.Log(_velocity.y.ToString());

        #region Camera Movement
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

             float mouseX = mouseDelta.x * _mouseSensitivity;
             float mouseY = mouseDelta.y * _mouseSensitivity;

             _xRotation -= mouseY;
             _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
             _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        #endregion

        #region Basic WASD movement
            float moveX = 0f;
            float moveZ = 0f;
            if (Keyboard.current.wKey.isPressed) moveZ = 1f;
            if (Keyboard.current.sKey.isPressed) moveZ = -1f;
            if (Keyboard.current.dKey.isPressed) moveX = 1f;
            if (Keyboard.current.aKey.isPressed) moveX = -1f;
            _movement = new Vector3(moveX, 0f, moveZ).normalized;
        #endregion


        if (moveZ != 1f && !_controller.isGrounded) _savedVelocityModifier = _crouchModifier;
        if (Keyboard.current.leftShiftKey.isPressed && _controller.isGrounded) _savedVelocityModifier = _sprintModifier; // Przytrzymanie Shift zwiększa prędkość
        if (Keyboard.current.leftCtrlKey.isPressed && _controller.isGrounded) _savedVelocityModifier = _crouchModifier;



        _movement *= _savedVelocityModifier;
        Vector3 moveDirection = transform.TransformDirection(_movement);
        _controller.Move(moveDirection * _speedFloat * Time.deltaTime);


        // --- 3. GRAWITACJA ---
        if (_controller.isGrounded) _velocity.y = -2f;
        else if (!_controller.isGrounded) _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        if (Physics.Raycast(transform.position, Vector3.up, _raycastLenght * transform.localScale.y, LayerMask.GetMask("Default"))) if (_velocity.y > 0) _velocity.y = (0 - _velocity.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _raycastLenght * transform.localScale.y);
    }

}