using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public static class MatchmakingService
{
    #region Events
    public static event Action MatchmakingCodeChanged;
    #endregion

    #region Public Properties
    public static bool Initialized { get; private set; }

    public static string MatchmakingCode {
        get => _matchmakingCode;
        private set
        {
            _matchmakingCode = value;
            MatchmakingCodeChanged?.Invoke();
        }
    }
    #endregion

    #region Private Variables
    private static string _matchmakingCode;

    private static UnityTransport _transport;
    #endregion

    public static void Initialize()
    {
        _transport = GameObject.FindObjectOfType<UnityTransport>();
        if (_transport == null)
        {
            throw new Exception("Could not find UnityTransport while initializing MatchmakingManager");
        }

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
                MatchmakingCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            }

            return success;
        } catch(Exception e)
        {
            return false;
        }
    }

    public static async Task<bool> TryJoinServer(string joinCode)
    {
        if (!Initialized)
        {
            return false;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

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
                MatchmakingCode = joinCode;
            }

            return success;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public static void LeaveServer()
    {
        if (!Initialized)
        {
            return;
        }

        NetworkManager.Singleton.Shutdown();

        MatchmakingCode = "";
    }

    public static string GetServerCode()
    {
        return MatchmakingCode;
    }
}