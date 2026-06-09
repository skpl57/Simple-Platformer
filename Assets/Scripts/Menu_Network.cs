using PurrNet;
using PurrNet.Modules;
using PurrNet.Transports;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Network : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _hostButton;

    [Header("Scene changer")]
    [PurrScene][SerializeField] private string _gameplaySceneName = "MainScene";

    [SerializeField] private Transform _networkNode;
    private NetworkManager _networkManager;
    private UDPTransport _transport;

    private void Start()
    {
        _networkManager = _networkNode.GetComponent<NetworkManager>();
        _transport = _networkNode.GetComponent<UDPTransport>();

        _networkManager.onServerConnectionState += OnServerConnectionStateChanged;

        _hostButton.onClick.AddListener(OnHostButtonClicked);
        _joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        _networkManager.StartHost();
    }

    private void OnServerConnectionStateChanged(ConnectionState state)
    {
        if (state == ConnectionState.Connected)
        {
            PurrSceneSettings settings = new()
            {
                isPublic = true,
                mode = LoadSceneMode.Additive
            };

            _networkManager.sceneModule.LoadSceneAsync(_gameplaySceneName, settings);
        }
    }

    private void OnJoinButtonClicked()
    {
        string targetIP = _ipInputField.text.Trim();
        if (string.IsNullOrEmpty(targetIP)) targetIP = "127.0.0.1";

        _transport.address = targetIP;
        _networkManager.StartClient();
    }
}
