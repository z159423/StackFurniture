using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Furnitures = new List<GameObject>();

    [Space]

    [SerializeField] private Transform FurniturePool;
    [SerializeField] private Transform FurnitureSpawnPosition;

    [Space]

    [SerializeField] private GameObject CurrentControllFurniture;
    [SerializeField] private FixedTouchField fixedTouchField;
    [SerializeField] private GameFlowController gameFlowController;
    [SerializeField] private LineRenderer furnitureDownLine;

    [Space]

    private Vector3 currentFurnitureCenterVec;

    private Vector3 originFurnitureSpawnPosition;


    [Space]

    public float furnitureDownSpeed = 2f;
    public float furnitureMoveSpeed = 1f;

    private TouchControls touchControls;

    public static FurnitureSpawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        originFurnitureSpawnPosition = FurnitureSpawnPosition.position;
    }

    private void Update()
    {
        if (CurrentControllFurniture != null && gameFlowController.gameStarted)
        {
            CurrentControllFurniture.transform.localPosition += (Vector3.down * Time.deltaTime * furnitureDownSpeed);

            //CurrentControllFurniture.transform.Translate(Vector3.down * Time.deltaTime * furnitureDownSpeed);

            //CurrentControllFurniture.transform.Translate(new Vector3(fixedTouchField.TouchDist.x,0,0) * Time.deltaTime * furnitureMoveSpeed);
            //CurrentControllFurniture.transform.Translate(new Vector3(0, 0, fixedTouchField.TouchDist.y) * Time.deltaTime * furnitureMoveSpeed);

            //CurrentControllFurniture.transform.position.x += fixedTouchField.TouchDist.x * Time.deltaTime * 10;
            //CurrentControllFurniture.transform.position.z += fixedTouchField.TouchDist.y * Time.deltaTime * 10;

            // if (gameFlowController.GetCameraViewPort() == 0)
            // {
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(fixedTouchField.TouchDist.x, 0, 0) * Time.deltaTime * furnitureMoveSpeed);
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(0, 0, fixedTouchField.TouchDist.y) * Time.deltaTime * furnitureMoveSpeed);
            // }
            // else if (gameFlowController.GetCameraViewPort() == 1)
            // {
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(-fixedTouchField.TouchDist.y, 0, 0) * Time.deltaTime * furnitureMoveSpeed);
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(0, 0, fixedTouchField.TouchDist.x) * Time.deltaTime * furnitureMoveSpeed);
            // }
            // else if (gameFlowController.GetCameraViewPort() == 2)
            // {
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(-fixedTouchField.TouchDist.x, 0, 0) * Time.deltaTime * furnitureMoveSpeed);
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(0, 0, -fixedTouchField.TouchDist.y) * Time.deltaTime * furnitureMoveSpeed);
            // }
            // else if (gameFlowController.GetCameraViewPort() == 3)
            // {
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(fixedTouchField.TouchDist.y, 0, 0) * Time.deltaTime * furnitureMoveSpeed);
            //     CurrentControllFurniture.transform.localPosition += (new Vector3(0, 0, -fixedTouchField.TouchDist.x) * Time.deltaTime * furnitureMoveSpeed);
            // }

            CurrentControllFurniture.transform.Translate(CameraManager.instance.GetCameraLookForwardDir() * fixedTouchField.TouchDist.y * Time.deltaTime * furnitureMoveSpeed);
            CurrentControllFurniture.transform.Translate(CameraManager.instance.GetCameraLookRightDir() * fixedTouchField.TouchDist.x * Time.deltaTime * furnitureMoveSpeed);

            // CurrentControllFurniture.transform.Translate(new Vector3(fixedTouchField.TouchDist.x, 0, fixedTouchField.TouchDist.y) * Time.deltaTime * furnitureMoveSpeed);


            currentFurnitureCenterVec = CurrentControllFurniture.GetComponent<MeshCollider>().bounds.center;
            //currentFurnitureCenterVec = CurrentControllFurniture.GetComponent<MeshFilter>().mesh.bounds.center;

            furnitureDownLine.SetPosition(0, currentFurnitureCenterVec);
            furnitureDownLine.SetPosition(1, new Vector3(currentFurnitureCenterVec.x, 0, currentFurnitureCenterVec.z));
        }
    }

    public void SpawnNewFurniture()
    {
        if (!gameFlowController.gameStarted)
            return;

        int furnitureNumber = Random.Range(0, Furnitures.Count);

        CurrentControllFurniture = Instantiate(Furnitures[furnitureNumber], FurnitureSpawnPosition.position, Quaternion.identity, FurniturePool);
        //CurrentControllFurniture.GetComponent<NonConvexMeshCollider>().Calculate();

        gameFlowController.AddNewFurniture(CurrentControllFurniture);

        CameraManager.instance.ChangeCameraTarget(CurrentControllFurniture.transform);
    }

    public void ClearAllGeneratedFurniture()
    {
        Transform[] childList = FurniturePool.GetComponentsInChildren<Transform>();

        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }

    public void GameStart()
    {
        furnitureDownLine.enabled = true;
    }

    public void GameEnd()
    {
        if (CurrentControllFurniture != null)
        {
            CurrentControllFurniture.GetComponent<Rigidbody>().useGravity = true;
            furnitureDownLine.enabled = false;
        }
    }

    public void RotateLeft()
    {
        if (CurrentControllFurniture != null)
        {
            //CurrentControllFurniture.transform.localRotation = Quaternion.Euler(CurrentControllFurniture.transform.localRotation.eulerAngles + new Vector3(0, 90, 0));

            //CurrentControllFurniture.transform.Rotate(new Vector3(0, 90, 0));
            CurrentControllFurniture.transform.RotateAround(currentFurnitureCenterVec, new Vector3(0, 90, 0), 90);
        }
    }

    public void RotateRight()
    {
        if (CurrentControllFurniture != null)
        {
            //CurrentControllFurniture.transform.localRotation = Quaternion.Euler(CurrentControllFurniture.transform.localRotation.eulerAngles + new Vector3(0, -90, 0));

            //CurrentControllFurniture.transform.Rotate(new Vector3(0, -90, 0));
            CurrentControllFurniture.transform.RotateAround(currentFurnitureCenterVec, new Vector3(0, -90, 0), -90);
        }
    }

    public void RotateUp()
    {
        if (CurrentControllFurniture != null)
        {
            //CurrentControllFurniture.transform.localRotation = Quaternion.Euler(CurrentControllFurniture.transform.localRotation.eulerAngles + new Vector3(90, 0, 0));

            //CurrentControllFurniture.transform.Rotate(new Vector3(90, 0, 0));
            CurrentControllFurniture.transform.RotateAround(currentFurnitureCenterVec, new Vector3(90, 0, 0), 90);
        }
    }

    public void RotateDown()
    {
        if (CurrentControllFurniture != null)
        {
            //CurrentControllFurniture.transform.localRotation = Quaternion.Euler(CurrentControllFurniture.transform.localRotation.eulerAngles + new Vector3(-90, 0, 0));

            //CurrentControllFurniture.transform.Rotate(new Vector3(-90, 0, 0));
            CurrentControllFurniture.transform.RotateAround(currentFurnitureCenterVec, new Vector3(-90, 0, 0), -90);
        }
    }

    public void ChangeFurnitureSpawnPosition(Vector3 position)
    {
        FurnitureSpawnPosition.position = FurnitureSpawnPosition.position + position;
    }

    public void ResetFurnitureSpawnPosition(Vector3 offset)
    {
        FurnitureSpawnPosition.position = originFurnitureSpawnPosition + offset;
    }
}
