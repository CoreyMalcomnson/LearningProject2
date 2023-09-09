using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    enum State
    {
        IDLE,

        MAIN_MENU,

        GAME,
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Spawn");
        SceneManager.LoadScene(2);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log("Despawn");
        SceneManager.LoadScene(1);
    }
}
