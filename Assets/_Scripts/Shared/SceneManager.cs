using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : NetworkBehaviour
{
    public static SceneManager Instance;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scenes.Game);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log("Despawn");
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scenes.MainMenu);
    }
}
