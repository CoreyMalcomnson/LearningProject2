using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private async void Awake()
    {
        await MatchmakingManager.Initialize();
    }
}
