using UnityEngine;

public class Jumping_Modifier : MonoBehaviour
{
    private Player_Movement _playerMovement;
    private Stamina_System _staminaSystem;

    private float _savedJumpingVelocityModifier = 1f;
    void Start()
    {
        _playerMovement = GetComponent<Player_Movement>();
        _staminaSystem = GetComponent<Stamina_System>();
    }
    private void Update()
    {
        _playerMovement.JumpingVelocityModifier = _staminaSystem.Stamina > 0f ? _savedJumpingVelocityModifier : _savedJumpingVelocityModifier * 0.75f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trampoline"))
        {
            _playerMovement.JumpingVelocityModifier = 5f;
            _savedJumpingVelocityModifier = 5f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trampoline"))
        {
            _playerMovement.JumpingVelocityModifier = 1f;
            _savedJumpingVelocityModifier = 1f;
        }
    }
}
