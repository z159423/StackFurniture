using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public List<VirtualCameras> virtualCameras = new List<VirtualCameras>();

    CinemachineVirtualCamera currentVirtualCamera;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] CinemachineVirtualCamera virtualCamera;


    public void ChangeCameraTarget(Transform target)
    {
        virtualCamera.Follow = target;
    }

    public void ChangeVirtualCamera(string tag)
    {
        if (currentVirtualCamera != null)
            currentVirtualCamera.Priority = 10;

        currentVirtualCamera = virtualCameras.Find(f => f.tag == tag).virtualCam;
        currentVirtualCamera.Priority = 9;
    }

    [System.Serializable]
    public class VirtualCameras
    {
        public CinemachineVirtualCamera virtualCam;
        public string tag;
    }
}
