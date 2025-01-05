using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private string NewGamelevel1 = "level1";
    public void NewGameButton()
    {
        SceneManager.LoadScene(NewGamelevel1);
    }
}
