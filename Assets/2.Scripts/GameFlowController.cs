using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Resources;


public class GameFlowController : MonoBehaviour
{
    [SerializeField] private FurnitureSpawnManager furnitureSpawnManager;
    [SerializeField] private FixedTouchField fixedTouchField;

    [Space]

    [SerializeField] private GameObject GameTitle;
    [SerializeField] private GameObject StartGamePanel;
    [SerializeField] private GameObject StartGameText;
    [SerializeField] private GameObject TouchToRestartText;
    [SerializeField] private GameObject GoToMainMenuPanel;
    [SerializeField] private GameObject RotateButtonBundle;
    [SerializeField] private GameObject CameraChangeViewPort;
    [SerializeField] private GameObject HeartPanel;
    [SerializeField] private GameObject PuaseButton;
    [SerializeField] private GameObject PauseBtn;
    [SerializeField] private GameObject ResumeBtn;

    [SerializeField] private GameObject OptionPanel;
    [SerializeField] private GameObject TutorialPanel;

    [Space]

    [SerializeField] private Button TutorialButton;
    [SerializeField] private Button OptionButton;
    [SerializeField] private Button SoundButton;
    [SerializeField] private Button VibrationButton;
    [SerializeField] private Sprite SoundOnSprite;
    [SerializeField] private Sprite SoundOffSprite;
    [SerializeField] private Sprite VibrationOnSprite;
    [SerializeField] private Sprite VibrationOffSprite;
    [SerializeField] private Sprite PauseButtonSprite;
    [SerializeField] private Sprite ResumeButtonSprite;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI TopScoreText;
    [SerializeField] private TextMeshProUGUI TopScoreText2;

    [SerializeField] private Camera camera;
    [SerializeField] private Light mainLight;
    [SerializeField] private Transform[] cameraPositions;
    [SerializeField] public Transform UIParent;


    private TouchControls touchControls;

    private int score = 0;
    private int currentCameraPosition = 0;
    private float currentCameraHeight = 0;

    public bool gameStarted = false;
    public LayerMask objectLayer;
    public Vector3 cameraOffect;

    private float originHeight = 0;
    private List<GameObject> CreatedFurniture = new List<GameObject>();

    public bool playSound = true;
    public bool playVibration = true;

    public bool isRevivedInThisStage = false;


    public static GameFlowController instance;

    private void Awake()
    {
        instance = this;

        touchControls = new TouchControls();

        Application.targetFrameRate = 60;

        if (!ES3.KeyExists("TryStage"))
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("FirstOpenGame");

            ES3.Save<int>("TryStage", 0);
        }

    }

    private void Start()
    {
        originHeight = camera.transform.position.y;
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    public void StartGame()
    {
        if (OptionPanel.activeSelf || TutorialPanel.activeSelf)
            return;

        CameraManager.instance.ResetCamera();

        gameStarted = true;

        GameTitle.SetActive(false);
        StartGamePanel.SetActive(false);
        StartGameText.SetActive(false);
        TouchToRestartText.SetActive(false);
        fixedTouchField.gameObject.SetActive(true);
        ScoreText.gameObject.SetActive(true);

        RotateButtonBundle.SetActive(true);
        CameraChangeViewPort.SetActive(true);
        //HeartPanel.SetActive(true);
        PuaseButton.SetActive(true);
        OptionButton.gameObject.SetActive(false);
        TutorialButton.gameObject.SetActive(false);

        furnitureSpawnManager.SpawnNewFurniture();

        CameraManager.instance.ChangeVirtualCamera("furniture");

        ES3.Save<int>("TryStage", ES3.Load<int>("TryStage") + 1);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("TryStage", "tryNum", (ES3.Load<int>("TryStage")));

        AdManager.instance.IrTimeTicking = true;

        TopScoreText2.text = SaveLoad.GetTopScore().ToString();
    }

    public void GameEnd()
    {
        gameStarted = false;

        fixedTouchField.gameObject.SetActive(false);
        TouchToRestartText.SetActive(true);
        GoToMainMenuPanel.SetActive(true);
        RotateButtonBundle.SetActive(false);
        PuaseButton.SetActive(false);
        // TopScoreText.gameObject.SetActive(true);
        GetTopScore();
        //HeartPanel.SetActive(false);

        CameraManager.instance.ChangeCameraTarget(null);

        Firebase.Analytics.FirebaseAnalytics.LogEvent("FailedStage", "score", (ES3.Load<int>("TryStage") + 1));

        isRevivedInThisStage = false;
    }

    public void GoToMainMenu()
    {
        CameraManager.instance.ChangeVirtualCamera("main");

        /*if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }*/

        currentCameraHeight = 0;
        ResetCameraPosition();
        furnitureSpawnManager.ResetFurnitureSpawnPosition(cameraOffect);

        TouchToRestartText.SetActive(false);
        GoToMainMenuPanel.SetActive(false);

        GameTitle.SetActive(true);
        StartGamePanel.SetActive(true);
        StartGameText.SetActive(true);
        // TopScoreText.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(false);
        CameraChangeViewPort.SetActive(false);
        OptionButton.gameObject.SetActive(true);
        TutorialButton.gameObject.SetActive(true);

        CreatedFurniture.Clear();

        SaveLoad.SetTopScore(score);
        ResetScore();

        furnitureSpawnManager.ClearAllGeneratedFurniture();

        AdManager.instance.IrTimeTicking = false;
        AdManager.instance.CallIrAds();

        isRevivedInThisStage = false;
    }

    public void AddScore(int num)
    {
        score += num;

        ScoreText.text = score.ToString();
    }

    public void GetTopScore()
    {
        TopScoreText.text = SaveLoad.GetTopScore().ToString();
    }

    public void ResetScore()
    {
        score = 0;

        ScoreText.text = score.ToString();
    }

    public void ChangeCameraPosition()
    {
        if (currentCameraPosition + 2 > cameraPositions.Length)
            currentCameraPosition = 0;
        else
            currentCameraPosition++;

        camera.transform.position = cameraPositions[currentCameraPosition].position + new Vector3(0, currentCameraHeight, 0) + cameraOffect;
        camera.transform.rotation = cameraPositions[currentCameraPosition].rotation;

        mainLight.transform.rotation = Quaternion.Euler(50, -50 + (currentCameraPosition * -90), 0);
    }

    public void ResetCameraPosition()
    {
        camera.transform.position = cameraPositions[currentCameraPosition].position + new Vector3(0, currentCameraHeight, 0) + cameraOffect;
        camera.transform.rotation = cameraPositions[currentCameraPosition].rotation;

        mainLight.transform.rotation = Quaternion.Euler(50, -50 + (currentCameraPosition * -90), 0);
    }

    public int GetCameraViewPort()
    {
        return currentCameraPosition;
    }

    public void CheckHeight()
    {
        int count = 5;

        if (CreatedFurniture.Count <= 5)
            count = CreatedFurniture.Count;

        for (int i = 0; i < count; i++)
        {

            var center = CreatedFurniture[CreatedFurniture.Count - i - 1].GetComponentInChildren<MeshCollider>().bounds.center;
            RaycastHit hit;
            var point = Physics.Raycast(center + (Vector3.up * 1000), Vector3.down, out hit, Mathf.Infinity, objectLayer);

            //Debug.LogError(CreatedFurniture[CreatedFurniture.Count - i - 1].gameObject.name);

            if (point)
            {
                //Debug.LogError(hit.point.y);

                if (currentCameraHeight < hit.point.y + cameraOffect.y)
                {
                    currentCameraHeight = hit.point.y;
                    camera.transform.position = new Vector3(camera.transform.position.x, originHeight + currentCameraHeight, camera.transform.position.z) + cameraOffect;

                    furnitureSpawnManager.ChangeFurnitureSpawnPosition(new Vector3(0, hit.point.y, 0));
                }
            }
        }

    }

    public void AddNewFurniture(GameObject furniture)
    {
        CreatedFurniture.Add(furniture);
    }

    public void GamePause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            // PuaseButton.GetComponent<Button>().image.sprite = PauseButtonSprite;
            PauseBtn.SetActive(true);
            ResumeBtn.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            // PuaseButton.GetComponent<Button>().image.sprite = ResumeButtonSprite;
            PauseBtn.SetActive(false);
            ResumeBtn.SetActive(true);

            Firebase.Analytics.FirebaseAnalytics.LogEvent("OnClickPuaseBtn");
        }

    }

    public bool GetGameState()
    {
        return gameStarted;
    }

    public void OptionOn()
    {
        OptionPanel.SetActive(!OptionPanel.activeSelf);

        TutorialPanel.SetActive(false);
    }

    public void SoundOnOff()
    {
        playSound = !playSound;

        if (playSound)
            SoundButton.image.sprite = SoundOnSprite;
        else
            SoundButton.image.sprite = SoundOffSprite;
    }

    public void ChangeSoundSetting(bool sound)
    {
        playSound = sound;

        if (sound)
            SoundButton.image.sprite = SoundOnSprite;
        else
            SoundButton.image.sprite = SoundOffSprite;
    }

    public void VibrationOnOff()
    {
        playVibration = !playVibration;

        if (playVibration)
            VibrationButton.image.sprite = VibrationOnSprite;
        else
            VibrationButton.image.sprite = VibrationOffSprite;
    }

    public void ChangeVibrationSetting(bool vibration)
    {
        playSound = vibration;

        if (vibration)
            VibrationButton.image.sprite = VibrationOnSprite;
        else
            VibrationButton.image.sprite = VibrationOffSprite;
    }

    public void TutorialOn()
    {
        if (!TutorialPanel.activeSelf)
            Firebase.Analytics.FirebaseAnalytics.LogEvent("TutorialOpen");
        else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("TutorialClose");

        TutorialPanel.SetActive(!TutorialPanel.activeSelf);

        OptionPanel.SetActive(false);

    }

    public void Revive()
    {
        FreezeCreatedFurnitures();

        isRevivedInThisStage = true;

        gameStarted = true;
    }

    void FreezeCreatedFurnitures()
    {
        foreach (var furniture in CreatedFurniture)
        {
            furniture.GetComponentInChildren<Furniture>().Freeze();
        }
    }

    public void TriggerInWater()
    {
        gameStarted = false;
        if (isRevivedInThisStage)
        {
            GameEnd();
            furnitureSpawnManager.GameEnd();
        }
        else
        {
            var prefab = Resources.Load<GameObject>("UI/ReviveUI");

            Instantiate(prefab, Vector3.zero, Quaternion.identity, UIParent);
        }

    }
}
