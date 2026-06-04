using UnityEngine;

public class Hand_Animations : MonoBehaviour
{
    [Header("Ręce")]
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    [SerializeField] private float _handReturnSpeed = 8f;

    private Vector3 _startedPositionLeft;
    private Vector3 _startedPositionRight;

    private bool _pointToPlayer;
    public bool PointToPlayer { get => _pointToPlayer; set => _pointToPlayer = value; }
    private void Start()
    {
        _startedPositionLeft = _leftHand.transform.localPosition;
        _startedPositionRight = _rightHand.transform.localPosition;
    }
    void Update()
    {
        if (PointToPlayer)
        {
            _rightHand.localRotation = Quaternion.Slerp(
                    _rightHand.localRotation,
                    Quaternion.Euler(90f, 0f, 0f),
                    _handReturnSpeed * Time.deltaTime
                );
            _rightHand.transform.localPosition = Vector3.Slerp(_rightHand.transform.localPosition, new Vector3(0.5f, -0.4f, 0.75f), _handReturnSpeed * Time.deltaTime);
        }
        else ResetHands();
    }
    public void ResetHands()
    {
        PointToPlayer = false;
        
        _rightHand.transform.localRotation = Quaternion.Slerp(_rightHand.localRotation, Quaternion.Euler(0f, 0f, 12f), _handReturnSpeed * Time.deltaTime);
        _rightHand.transform.localPosition = Vector3.Slerp(_rightHand.transform.localPosition, _startedPositionRight, _handReturnSpeed * Time.deltaTime);
     
        _leftHand.transform.localRotation = Quaternion.Slerp(_leftHand.localRotation, Quaternion.Euler(0f, 0f, 12f), _handReturnSpeed * Time.deltaTime);
        _leftHand.transform.localPosition = Vector3.Slerp(_leftHand.transform.localPosition, _startedPositionLeft, _handReturnSpeed * Time.deltaTime);
    }
}
