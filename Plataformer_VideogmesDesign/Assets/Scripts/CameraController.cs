using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    public float cameraZpos = -10;
    public float cameraXOffset = 5f;
    public float cameraYOffset = 1f;

    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;

    private Transform _camera;
    private PlayerController _playerController;


    void Start()
    {
        //Reference to the player
        if(!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        _playerController = player.GetComponent<PlayerController>(); //Reference to PlayerController script

        _camera = Camera.main.transform;

        _camera.position = new Vector3(
            player.transform.position.x + cameraXOffset,
            player.transform.position.y + cameraYOffset,
            player.transform.position.z + cameraZpos
            );


    }


    void Update()
    {
        if (_playerController.isFacingRight)
        {
            _camera.position = new Vector3(
                Mathf.Lerp(_camera.position.x, player.transform.position.x + cameraXOffset, horizontalSpeed * Time.deltaTime),
                Mathf.Lerp(_camera.position.y, player.transform.position.y + cameraYOffset, cameraYOffset = verticalSpeed * Time.deltaTime),
                cameraZpos
                );
        }
        else
        {
            _camera.position = new Vector3(
                Mathf.Lerp(_camera.position.x, player.transform.position.x - cameraXOffset, horizontalSpeed * Time.deltaTime),
                Mathf.Lerp(_camera.position.y, player.transform.position.y + cameraYOffset, cameraYOffset = verticalSpeed * Time.deltaTime),
                cameraZpos
                );
        }
    }
}
