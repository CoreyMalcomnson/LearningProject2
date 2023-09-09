using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchmakingCodeGui : MonoBehaviour
{
    private MatchmakingCodeGui _instance;

    [SerializeField] private TMP_Text matchmakingCodeText;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(_instance);
            return;
        } else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        OnMatchmakingCodeChanged();
        MatchmakingService.MatchmakingCodeChanged += OnMatchmakingCodeChanged;
    }

    private void OnDestroy()
    {
        MatchmakingService.MatchmakingCodeChanged -= OnMatchmakingCodeChanged;
    }

    private void OnMatchmakingCodeChanged()
    {
        matchmakingCodeText.text = MatchmakingService.MatchmakingCode;
    }
}
