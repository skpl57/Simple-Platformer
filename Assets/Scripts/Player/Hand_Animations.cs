using System;
using UnityEngine;

public class Hand_Animations : MonoBehaviour
{
    [Header("Ręce")]
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    [SerializeField] private float _handReturnSpeed = 8f;
    [SerializeField] private Camera _camera;

    private Vector3 _startedPositionLeft;
    private Vector3 _startedPositionRight;

    private bool _pointToPlayer;
    private bool _climbing;
    public bool PointToPlayer { get => _pointToPlayer; set => _pointToPlayer = value; }
    public bool Climbing { get => _climbing; set => _climbing = value; }
    private void Start()
    {
        _startedPositionLeft = _leftHand.transform.localPosition;
        _startedPositionRight = _rightHand.transform.localPosition;
    }
    void Update()
    {
        if (PointToPlayer) PointPlayer();
        else if (!Climbing) ResetHands();
    }

    private void PointPlayer()
    {
        _rightHand.localRotation = Quaternion.Slerp(
                    _rightHand.localRotation,
                    Quaternion.Euler(90f, 0f, 0f),
                    _handReturnSpeed * Time.deltaTime
                );
        _rightHand.transform.localPosition = Vector3.Slerp(_rightHand.transform.localPosition, new Vector3(0.5f, -0.4f, 0.75f), _handReturnSpeed * Time.deltaTime);
    }
    public void HoldingEdge(Vector3 edgePosition, Vector3 wallNormal)
    {
        Quaternion targetRotation = Quaternion.LookRotation(wallNormal, Vector3.up);
        Vector3 baseHandOffset = (wallNormal * 0.05f) + (Vector3.up * 0.02f);

        Vector3 localRight = Quaternion.LookRotation(wallNormal, Vector3.up) * Vector3.right;

        Vector3 rightHandTargetPos = edgePosition + baseHandOffset - (localRight * 0.5f);
        Vector3 leftHandTargetPos = edgePosition + baseHandOffset + (localRight * 0.5f);

        _rightHand.transform.position = Vector3.Slerp(_rightHand.transform.position, rightHandTargetPos, _handReturnSpeed * Time.deltaTime);
        _leftHand.transform.position = Vector3.Slerp(_leftHand.transform.position, leftHandTargetPos, _handReturnSpeed * Time.deltaTime);

        _rightHand.transform.rotation = Quaternion.Slerp(_rightHand.transform.rotation, Quaternion.Euler(0f, 0f, 0f), (_handReturnSpeed / 2f) * Time.deltaTime);
        _leftHand.transform.rotation = Quaternion.Slerp(_leftHand.transform.rotation, Quaternion.Euler(0f, 0f, 0f), (_handReturnSpeed / 2f) * Time.deltaTime);
    }

    private void ResetHands()
    {
        PointToPlayer = false;
        
        _rightHand.transform.rotation = Quaternion.Slerp(_rightHand.rotation, Quaternion.Euler(0f, 0f, 12f), _handReturnSpeed * Time.deltaTime);
        _rightHand.transform.localPosition = Vector3.Slerp(_rightHand.transform.localPosition, _startedPositionRight, _handReturnSpeed * Time.deltaTime);
     
        _leftHand.transform.rotation = Quaternion.Slerp(_leftHand.rotation, Quaternion.Euler(0f, 0f, 12f), _handReturnSpeed * Time.deltaTime);
        _leftHand.transform.localPosition = Vector3.Slerp(_leftHand.transform.localPosition, _startedPositionLeft, _handReturnSpeed * Time.deltaTime);
    }
}
