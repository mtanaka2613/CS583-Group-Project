using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<int> levelsPlayed = new List<int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance= this; 
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        int randomLevel;
        //prevents a random level from being selected again
        do
        {
            randomLevel = Random.Range(2, 8);
        }
        while (levelsPlayed.Contains(randomLevel));
        levelsPlayed.Add(randomLevel);

        SceneManager.LoadScene(randomLevel);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Boolean hasCompletedAllLevels()
    {
        // if 6 levels have been completed then the player has completed all the levels, so return true
        if (levelsPlayed.Count == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
