using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;

public class FirebaseManager : MonoBehaviour
{
    FirebaseApp _app;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;

                FirebaseAnalytics.LogEvent("test");

                FirebaseAnalytics.LogEvent("param_test_int", "IntParam", 111);

                FirebaseAnalytics.LogEvent("param_test_int", "FloatParam", 2.22f);

                FirebaseAnalytics.LogEvent("param_test_int", "StringParam", "TEXT");

                FirebaseAnalytics.LogEvent("param_test_array", new Parameter(FirebaseAnalytics.ParameterCharacter, "warrior"),
                new Parameter(FirebaseAnalytics.ParameterLevel, 5));
            }
            else
            {
                Debug.LogError("Could not reslove all Firebase Dependencies : " + task.Result);
            }
        });
    }
}
