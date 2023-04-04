using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Localization.Settings;


public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    private void Awake()
    {
        instance = this;

        IEnumerator LocalInit()
        {
            yield return LocalizationSettings.InitializationOperation;

            if (Application.systemLanguage == SystemLanguage.English)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            }
            else if (Application.systemLanguage == SystemLanguage.Korean)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
            }
            else
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            }
        }

        StartCoroutine(LocalInit());
    }

    public void SaveSetting()
    {
        SaveData data = RoadData();

        data.soundPlay = GameFlowController.instance.playSound;
        data.vibrationPlay = GameFlowController.instance.playVibration;

        SaveDate(data);
    }

    public void RoadSetting()
    {
        SaveData data = RoadData();

        GameFlowController.instance.ChangeSoundSetting(data.soundPlay);
        GameFlowController.instance.ChangeVibrationSetting(data.vibrationPlay);
    }

    public void SaveScore(int score)
    {
        SaveData data = RoadData();

        data.TopScore = score;

        SaveDate(data);
    }

    public static int GetTopScore()
    {
        SaveData data = RoadData();

        return data.TopScore;
    }

    public static void SetTopScore(int score)
    {
        SaveData data = RoadData();

        if (data.TopScore < score)
            data.TopScore = score;

        SaveDate(data);
    }

    public static SaveData RoadData()
    {
        string path = Application.persistentDataPath + "/saveData";

        Debug.Log(path);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData settingData = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return settingData;
        }
        else
        {
            Debug.Log("���̺긦 ���ҷ��Խ��ϴ�. " + path + "\n���ο� ���̺� ������ �����մϴ�.");

            SaveData settingData = new SaveData();

            return settingData;
        }
    }

    public static void SaveDate(SaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/saveData";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }
}
