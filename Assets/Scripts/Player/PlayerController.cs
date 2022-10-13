using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput _input;

    void Start()
    {
        TryGetComponent(out _input);
    }
}
