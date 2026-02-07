using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Transform _player;

    public Vector3 offset = new Vector3(0, 0, -10); // 플레이어와의 거리 간격
    public float smoothTime = 0.3f; // 카메라가 도달하는 지연 시간 (낮을수록 빠름)

    private Vector3 velocity = Vector3.zero;

    public void InitCameraComp(PlayerCharacter player)
    {
        _player = player.transform;
    }

    void LateUpdate()
    {
        if (_player != null)
        {
            Vector3 targetPosition = _player.position + offset;

            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
