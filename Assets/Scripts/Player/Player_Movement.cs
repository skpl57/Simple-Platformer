using PurrNet;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player_Movement : NetworkIdentity
{
    [Header("Ruch i Fizyka")]
    [SerializeField] private float _speedFloat = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _sprintModifier = 2f;
    [SerializeField] private float _crouchModifier = 0.5f;
    [SerializeField] private float _normalJumpHeight = 2f;
    [SerializeField] private float _headHeight = 1.1f;
    [SerializeField] private Transform _body;


    private CharacterController _controller;
    private Hand_Animations _handAnimations;
    private Stamina_System _staminaSystem;

    private float _wallCheckDistance = 1f;
    private float _movingVelocityModifier = 1f;
    private float _jumpingVelocityModifier = 1f;

    private Vector3 _velocity;
    private Vector3 _movement = Vector3.zero;

    private Vector3 _boxPosition;
    private Vector3 _boxHalfExtents;
    private float _boxWidthModifier = 0.7f;
    private float _boxSide;

    private float _jumpStaminaCost = 15f;
    private float _reduceJumpHeight = 1.4f;

    private bool _isClimbing = false;
    private bool _canJump = true;
    private bool _canSprint = true;
    public float SprintModifier => _sprintModifier;
    public float CrouchModifier => _crouchModifier;
    public float MovingVelocityModifier => _movingVelocityModifier;
    public bool CanJump => _canJump;
    public float JumpingVelocityModifier { get => _jumpingVelocityModifier; set => _jumpingVelocityModifier = value; }
    public bool CanSprint { get => _canSprint; set => _canSprint = value; }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _handAnimations = GetComponent<Hand_Animations>();
        _staminaSystem = GetComponent<Stamina_System>();

        _boxSide = _controller.radius * _boxWidthModifier;
        _boxHalfExtents = new Vector3(_boxSide, 0.02f, _boxSide);
    }

    void Update()
    {
        if (!isOwner) return;

        _boxPosition = transform.position + new Vector3(0, _headHeight * _body.localScale.y, 0);
        bool isCeilingAbove = Physics.CheckBox(_boxPosition, _boxHalfExtents, transform.rotation, LayerMask.GetMask("Default"));
        float _jumpHeight = _normalJumpHeight;
        
        CalculateWallClimbing();

        if (!Physics.Raycast(transform.position + Vector3.up / 2f, Vector3.up, out RaycastHit hit, (_body.localScale.y == 0.5f ? 0.6f : 1.1f)))
        {
            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                _controller.height = 1.0f;
                _controller.center = new Vector3(0f, 0.1f, 0f);
                _body.localScale = new Vector3(1f, 0.5f, 1f);
                _jumpHeight = CanSprint ? _normalJumpHeight / 2f : _normalJumpHeight / _reduceJumpHeight / 2f;
            }
            else
            {
                _controller.height = 2.0f;
                _controller.center = new Vector3(0f, 0f, 0f);
                _body.localScale = new Vector3(1f, 1f, 1f);
                _jumpHeight = CanSprint ? _normalJumpHeight : _normalJumpHeight / _reduceJumpHeight;
            }
            if (_controller.isGrounded) _movingVelocityModifier = 1f;
            if (Keyboard.current.spaceKey.isPressed && _canJump)
            {
                if(JumpingVelocityModifier > 1f) _staminaSystem.ReduceStamina(_jumpStaminaCost + (CanSprint ? 0 : 100));
                _velocity.y = Mathf.Sqrt(-_jumpHeight * _gravity * 1.5f * JumpingVelocityModifier);
            }
        }

        CalculateBasicMovement();

        if (_controller.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        else if (!_canJump) _velocity.y += _gravity * Time.deltaTime;
        //if(_velocity.y < -2f) { Debug.Log($"Velocity {_velocity.y}, is dead fall: {_velocity.y < -20}"); }
        if (isCeilingAbove && _velocity.y > 0) _velocity.y = 0f;

        Vector3 horizontalMove = transform.TransformDirection(_movement) * _speedFloat;
        Vector3 finalMovement = (horizontalMove + _velocity) * Time.deltaTime;
        _controller.Move(finalMovement);
    }

    private void CalculateBasicMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;


        if (Keyboard.current.wKey.isPressed) moveZ = 1f;
        if (Keyboard.current.sKey.isPressed) moveZ = -1f;
        if (Keyboard.current.dKey.isPressed) moveX = 1f;
        if (Keyboard.current.aKey.isPressed) moveX = -1f;

        if (moveZ != 1f && !_controller.isGrounded) _movingVelocityModifier = _crouchModifier;

        if (Keyboard.current.leftShiftKey.isPressed && _controller.isGrounded && _body.localScale.y != 0.5f && CanSprint) _movingVelocityModifier = _sprintModifier;
        if ((Keyboard.current.leftCtrlKey.isPressed && _controller.isGrounded) || _body.localScale.y == 0.5f) _movingVelocityModifier = _crouchModifier;

        if(_isClimbing) _movingVelocityModifier = 1f;

        _movement = new Vector3(moveX, 0f, moveZ).normalized;
        _movement *= _movingVelocityModifier;
    }
    private void CalculateWallClimbing()
    {
        RaycastHit wall;
        RaycastHit topOfTheEdge;

        bool canWallClimb = Physics.Raycast(transform.position + Vector3.up / 2f, transform.forward, out wall, _wallCheckDistance * _body.localScale.y, LayerMask.GetMask("Default")) &&
                             !Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, _wallCheckDistance * _body.localScale.y, LayerMask.GetMask("Default"));
        _canJump = canWallClimb || _controller.isGrounded;
        _isClimbing = canWallClimb;
        if (canWallClimb)
        {
            _velocity.y = 0f;
            

            if (Physics.Raycast(transform.position + Vector3.up * 1.5f + transform.forward * wall.distance, Vector3.down, out topOfTheEdge, 1.5f, LayerMask.GetMask("Default")))
            {
                _handAnimations.HoldingEdge(topOfTheEdge.point, wall.normal);
                _handAnimations.Climbing = true;
            }
        }
        else _handAnimations.Climbing = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawLine(transform.position + Vector3.up / 2f, transform.position + Vector3.up * (_body.localScale.y == 0.5f ? 1.5f : 1f));

        Gizmos.color = Color.deepPink;
        Gizmos.DrawLine(transform.position + Vector3.up * 1.5f + transform.forward * _wallCheckDistance * _body.localScale.y
                        , transform.position + (Vector3.up / 2f) + transform.forward * _wallCheckDistance * _body.localScale.y);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up / 2f, transform.position + (Vector3.up / 2f) + transform.forward * _wallCheckDistance * _body.localScale.y);
        Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + Vector3.up * 1.5f + transform.forward * _wallCheckDistance * _body.localScale.y);

        if (Physics.CheckBox(_boxPosition, _boxHalfExtents, transform.rotation, LayerMask.GetMask("Default")))
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(_boxPosition, transform.rotation, _body.localScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, _boxHalfExtents);
        Gizmos.matrix = Matrix4x4.identity;
    }
}