using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerCodeGui : MonoBehaviour
{
    [SerializeField] private TMP_Text serverCodeText;

    private void Awake()
    {
        OnServerCodeChanged();
        MatchmakingManager.ServerCodeChanged += OnServerCodeChanged;
    }

    private void OnDestroy()
    {
        MatchmakingManager.ServerCodeChanged -= OnServerCodeChanged;
    }

    private void OnServerCodeChanged()
    {
        serverCodeText.text = MatchmakingManager.ServerCode;
    }
}
