using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerInputManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += (a) => { HUD.Instance.ImgControls.enabled = false; };
    }
}
