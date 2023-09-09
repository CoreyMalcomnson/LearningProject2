using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance;

    [SerializeField] private UnityTransport transport;

    private string _serverCode;
    private bool _isAuthenticated;

    private async void Awake()
    {
        Instance = this;

        await Authenticate();
    }

    private async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _isAuthenticated = true;
    }

    public async void StartServer()
    {
        if (!_isAuthenticated)
        {
            return;
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(30);
       
        _serverCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        
        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
        
        NetworkManager.Singleton.StartHost();
    }

    public async void JoinServer(string code)
    {
        if (!_isAuthenticated)
        {
            return;
        }

        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
        
        transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);
        
        NetworkManager.Singleton.StartClient();
    }

    public string GetServerCode()
    {
        return _serverCode;
    }
}