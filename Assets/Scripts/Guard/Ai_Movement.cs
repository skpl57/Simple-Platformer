using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ai_Movement : MonoBehaviour
{
    [Header("Działanie AI")]
    [SerializeField] private float _waypointTolerance = 1f;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private Transform _parent;
    private List<Transform> _wayPoints = new List<Transform>();
    private NavMeshAgent _agent;
    private int _currentWaypointIndex = 0;

    [Header("Obracanie Kostki")]
    [SerializeField] private Transform _cube;
    private float _rotationSpeed = 200f;

    private List<Transform> _playersInCollider = new List<Transform>();

    void Start()
    {
        foreach (Transform child in _parent)
        {
            _wayPoints.Add(child);
        }

        _agent = GetComponent<NavMeshAgent>();

        if (_wayPoints.Count > 0)
        {
            _agent.SetDestination(_wayPoints[0].position);
        }
    }

    void Update()
    {
        Transform targetPlayer = GetNearestVisiblePlayer();

        if (targetPlayer != null)
        {
            _agent.speed = 5f;
            _agent.SetDestination(targetPlayer.position);
        }
        else
        {
            Patrol();
        }
        _cube.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);
    }

    private void Patrol()
    {
        _agent.speed = 3.5f;
        _agent.SetDestination(_wayPoints[_currentWaypointIndex].position);

        float distance = Vector3.Distance(transform.position, _wayPoints[_currentWaypointIndex].position);

        if (distance < _waypointTolerance)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _wayPoints.Count;
            _agent.SetDestination(_wayPoints[_currentWaypointIndex].position);
        }
    }

    private Transform GetNearestVisiblePlayer()
    {
        Transform closestPlayer = null;
        float minDistance = 20;

        foreach (Transform player in _playersInCollider)
        {
            if (player == null) continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer + 1f))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    if (hit.transform.TryGetComponent<Player_Movement>(out Player_Movement movement))
                    {
                        movement.SlowingModifier = distanceToPlayer < 3;
                    }

                    if (distanceToPlayer < minDistance)
                    {
                        minDistance = distanceToPlayer;
                        closestPlayer = player;
                    }
                }
            }
        }

        return closestPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_playersInCollider.Contains(other.transform))
        {
            _playersInCollider.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playersInCollider.Remove(other.transform);
        }
    }
}
