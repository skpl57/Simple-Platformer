using PurrNet;
using PurrNet.Modules;
using PurrNet.Transports;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Menu_Network : NetworkIdentity
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button hostButton;

    [Header("Scene changer")]
    [PurrScene][SerializeField] private string gameplaySceneName;

    [SerializeField] private Transform _networkNode;
    private NetworkManager _networkManager;
    private UDPTransport _transport;

    private void Start()
    {
        _networkManager = _networkNode.GetComponent<NetworkManager>();
        _transport = _networkNode.GetComponent<UDPTransport>();


        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        PurrSceneSettings settings = new()
        {
            isPublic = true,
            mode = LoadSceneMode.Additive
        };
        _networkManager.sceneModule.LoadSceneAsync(gameplaySceneName, settings);
    }

    private void OnJoinButtonClicked()
    {
        string targetIP = ipInputField.text.Trim();

        if (string.IsNullOrEmpty(targetIP)) targetIP = "127.0.0.1";

        _transport.address = targetIP;
        ChangeScene();
    }

    [ServerRpc(requireOwnership: false)]
    private void ChangeScene(RPCInfo info = default)
    {
        Debug.Log("Działa");
        var scene = SceneManager.GetSceneByName(gameplaySceneName);
        if (!scene.isLoaded) return;

        if (_networkManager.sceneModule.TryGetSceneID(scene, out SceneID sceneID)) 
        {
            _networkManager.scenePlayersModule.AddPlayerToScene(info.sender, sceneID);
        }
    }
}
