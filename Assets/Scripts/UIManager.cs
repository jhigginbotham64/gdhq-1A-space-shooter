using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
  [SerializeField]
  private Text _scoreDisplay;
  [SerializeField]
  private Sprite[] _livesSprites;
  [SerializeField]
  private Image _livesImage;
  [SerializeField]
  private Text _gameOverText;
  [SerializeField]
  private Text _restartInstructionText;
  [SerializeField]
  private string _scoreDisplayPrefix = "Score: ";
  [SerializeField]
  private int _score = 0;

  void Start()
  {
    UpdateScoreDisplay();
  }

  void Update()
  {
    if (_restartInstructionText.enabled && Input.GetKeyDown(KeyCode.R))
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
  }

  private void UpdateScoreDisplay()
  {
    _scoreDisplay.text = _scoreDisplayPrefix + _score;
  }

  public void OnPlayerScore(GameEventArgs args)
  {
    _score += args.iValue;
    UpdateScoreDisplay();
  }

  public void OnPlayerLivesChanged(GameEventArgs args)
  {
    _livesImage.sprite = _livesSprites[args.iValue];

    if (args.iValue == 0)
    {
      _gameOverText.enabled = true;
      _restartInstructionText.enabled = true;
    }
  }
}
