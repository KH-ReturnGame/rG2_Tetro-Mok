using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    private NetworkList<ulong> lobbyPlayers; // 로비에 있는 플레이어 ID 목록
    private Dictionary<ulong, string> playerRoles; // 플레이어 역할 저장
    public static LobbyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lobbyPlayers = new NetworkList<ulong>();
        playerRoles = new Dictionary<ulong, string>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            lobbyPlayers.Add(clientId);
            playerRoles[clientId] = "None"; // 기본 역할
            Debug.Log($"플레이어 {clientId}가 로비에 참여했습니다.");
            UpdateLobbyClientRpc();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            lobbyPlayers.Remove(clientId);
            playerRoles.Remove(clientId);
            Debug.Log($"플레이어 {clientId}가 로비에서 나갔습니다.");
            UpdateLobbyClientRpc();
        }
    }

    [ClientRpc]
    private void UpdateLobbyClientRpc()
    {
        Debug.Log($"로비 업데이트 - 현재 플레이어 수: {lobbyPlayers.Count}");
        // UI 업데이트 로직 추가 가능
    }

    // 게임 시작
    public void StartGame()
    {
        if (IsServer) NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}