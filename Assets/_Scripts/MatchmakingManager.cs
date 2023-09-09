using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public static class MatchmakingManager
{

    public static event Action ServerCodeChanged;

    public static string ServerCode {
        get => _serverCode;
        private set
        {
            _serverCode = value;
            ServerCodeChanged?.Invoke();
        }
    }

    private static string _serverCode;

    public static bool Initialized { get; private set; }

    private static UnityTransport _transport;

    public static async Task Initialize()
    {
        _transport = UnityEngine.GameObject.FindObjectOfType<UnityTransport>();
        
        if (_transport == null)
        {
            Debug.LogError("Could not find UnityTransport while initializing MatchmakingManager");
            return;
        }

        //
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Initialized = true;
    }

    public static async Task<bool> TryHostServer()
    {
        if (!Initialized)
        {
            return false;
        }

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(30);
            
            _transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            bool success = NetworkManager.Singleton.StartHost();

            if (success)
            {
                ServerCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            }

            return success;
        } catch(Exception e)
        {
            return false;
        }
    }

    public static async Task<bool> TryJoinServer(string serverCode)
    {
        if (!Initialized)
        {
            return false;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(serverCode);

            _transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            bool success = NetworkManager.Singleton.StartClient();

            if (success)
            {
                ServerCode = serverCode;
            }

            return success;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public static string GetServerCode()
    {
        return ServerCode;
    }
}