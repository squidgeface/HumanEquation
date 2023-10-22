using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _controllerDeadZone = 0.1f;
    [SerializeField] private float _gamepadRotateSmoothing = 1000f;

    [SerializeField] private bool _isGamePad;

    private Vector3 _playerVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
