using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] TutorialWindows;

    private int currentTutorialWindow = 0;
    
    public void PreviousWindow()
    {
        try
        {
            if (TutorialWindows[currentTutorialWindow - 1] != null)
            {
                TutorialWindows[currentTutorialWindow].SetActive(false);
                TutorialWindows[currentTutorialWindow - 1].SetActive(true);
                currentTutorialWindow--;
            }
        }
        catch(System.IndexOutOfRangeException ex)
        {

        }
    }

    public void NextWindow()
    {
        try
        {
            if (TutorialWindows[currentTutorialWindow + 1] != null)
            {
                TutorialWindows[currentTutorialWindow].SetActive(false);
                TutorialWindows[currentTutorialWindow + 1].SetActive(true);
                currentTutorialWindow++;
            }
        }
        catch (System.IndexOutOfRangeException ex)
        {

        }
    }

}
