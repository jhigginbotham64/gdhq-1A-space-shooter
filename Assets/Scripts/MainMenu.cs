using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public void NewGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // +1 for next, only works if Build Settings are properly configured
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
  }
}
