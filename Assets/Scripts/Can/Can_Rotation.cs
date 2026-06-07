using UnityEngine;

public class Can_Rotation : MonoBehaviour
{
    private float _rotationSpeed = 150f;
    private float _amplitude = 0.25f;
    private float _frequency = 2f;
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
        float newY = _startPosition.y + Mathf.Sin(Time.time * _frequency) * _amplitude;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
}
