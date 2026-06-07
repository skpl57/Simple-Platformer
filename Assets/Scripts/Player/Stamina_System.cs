using UnityEngine;
using UnityEngine.UI;

public class Stamina_System : MonoBehaviour
{
    [SerializeField] private Image _staminaBar;
    [SerializeField] private Image _staminaBarBackground;
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _usageRate = 10f;
    private float _stamina = 100f;
    private float _visualStamina = 100f;
    private float _regenerationRate = 30f;
    private float _regenerationDelay = 4f;

    private Player_Movement _playerMovement;

    public float Stamina => _stamina;
    private float _timeSinceLastSprint = 0f;
    private bool _canRegenerate = false;
    private bool _canUseStamina = true;
    void Start()
    {
        _playerMovement = GetComponent<Player_Movement>();
        _visualStamina = _stamina;
    }

    void Update()
    {
        if (_stamina <= 0f)
        {
            _stamina = 0f;
            _playerMovement.CanSprint = false;
            _canUseStamina = false;
        }

        if (_playerMovement.MovingVelocityModifier > 1f && _canUseStamina)
        {
            _stamina -= Time.deltaTime * _usageRate;
            _timeSinceLastSprint = 0f;
            _canRegenerate = false;
        }


        if(_playerMovement.MovingVelocityModifier <= 1f && !_canRegenerate)
        {
            _timeSinceLastSprint += Time.deltaTime;
            if(_timeSinceLastSprint >= _regenerationDelay) _canRegenerate = true;
        }

        if (_canRegenerate)
        {
            _timeSinceLastSprint = 0f;
            _stamina = Mathf.Min(_stamina + Time.deltaTime * _regenerationRate, _maxStamina);
        }

        if (_stamina == _maxStamina && !_canUseStamina)
        {
            _canUseStamina = true;
            _playerMovement.CanSprint = true;
        }

        if (_stamina > 0f && _canUseStamina) _playerMovement.CanSprint = true;

        ChangeBarLook();


    }
    public void AddStamina(float amount)
    {
        _stamina = Mathf.Min(_stamina + amount, _maxStamina);
    }
    public void ReduceStamina(float amount)
    {
        _timeSinceLastSprint = 0f;
        _stamina = Mathf.Max(_stamina - amount, 0f);
        _canRegenerate = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("StaminaPickup"))
        {
            AddStamina(50f);
            Destroy(other.gameObject);
        }
    }
    private void ChangeBarLook()
    {
        _visualStamina = Mathf.Lerp(_visualStamina, _stamina, Time.deltaTime * 10.0f);
        _staminaBar.fillAmount = _visualStamina / _maxStamina;

        if (_canUseStamina)
        {
            _staminaBar.color = new Color32(37, 120, 224, 255);
            _staminaBarBackground.color = new Color32(30, 128, 140, 40);
        }
        else
        {
            _staminaBar.color = new Color32(204, 45, 32, 255);
            _staminaBarBackground.color = new Color32(140, 39, 30, 40);
        }

    }
}
