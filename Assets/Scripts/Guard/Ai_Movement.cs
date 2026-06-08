using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ai_Movement : MonoBehaviour
{
    [Header("Działanie AI")]
    [SerializeField] private float _waypointTolerance = 1f;
    [SerializeField] private Transform _parent;

    [Header("Obracanie Kostki")]
    [SerializeField] private Transform _cube;
    [SerializeField] private float _rotationSpeed = 150f;

    private int _currentWaypointIndex = 0;
    private NavMeshAgent _agent;
    private List<Transform> _wayPoints = new List<Transform>();
    void Start()
    {
        foreach(Transform child in _parent)
        {
            _wayPoints.Add(child);
        }

        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(_wayPoints[0].position);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, _wayPoints[_currentWaypointIndex].position);

        if (distance < _waypointTolerance)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _wayPoints.Count;
            _agent.SetDestination(_wayPoints[_currentWaypointIndex].position);
        }
        _cube.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);
    }
}
