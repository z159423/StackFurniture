using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public List<VirtualCameras> virtualCameras = new List<VirtualCameras>();

    public CinemachineFreeLook freecam;
    public Slider cameraRotateSlider;

    CinemachineVirtualCamera currentVirtualCamera;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] CinemachineVirtualCamera virtualCamera;


    public void ChangeCameraTarget(Transform target)
    {
        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;

        freecam.Follow = target;
        // freecam.LookAt = target;
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

    public void RotateCamera()
    {
        // currentVirtualCamera.transform.RotateAround(transform.position, new Vector3(0, (cameraRotateSlider.value * 360) - 180, 0), Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, (cameraRotateSlider.value * 360) - 180, 0);
    }

    public Vector3 GetCameraLookForwardDir()
    {
        return new Vector3(currentVirtualCamera.gameObject.transform.forward.x, 0, currentVirtualCamera.gameObject.transform.forward.z);
        // return (new Vector3(currentVirtualCamera.transform.position.x, 0, currentVirtualCamera.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
    }

    public Vector3 GetCameraLookRightDir()
    {
        return new Vector3(currentVirtualCamera.gameObject.transform.right.x, 0, currentVirtualCamera.gameObject.transform.right.z);
        // return currentVirtualCamera.gameObject.transform.right;
    }
}
