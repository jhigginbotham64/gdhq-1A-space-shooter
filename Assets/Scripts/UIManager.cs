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
  private GameObject _shipHealthDisplay;
  private Component[] _shipHealthBlocks;
  [SerializeField]
  private GameObject _shieldHealthDisplay;
  private Component[] _shieldHealthBlocks;
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
    _shipHealthBlocks = _shipHealthDisplay.GetComponentsInChildren(typeof(CanvasRenderer));
    _shieldHealthBlocks = _shieldHealthDisplay.GetComponentsInChildren(typeof(CanvasRenderer));
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

  public void OnPlayerSpawned(GameEventArgs args)
  {
    foreach(CanvasRenderer c in _shipHealthBlocks)
    {
      c.gameObject.SetActive(true);
    }
    return;
  }

  public void OnPlayerScore(GameEventArgs args)
  {
    _score += args.iValue;
    UpdateScoreDisplay();
  }

  private int _playerHitsTaken = 0;
  private bool _playerShieldActive = false;
  private int _playerShieldHitsTaken = 0;

  public void OnPlayerDamageTaken(GameEventArgs args)
  {
    if (_shieldHealthDisplay.activeInHierarchy)
    {
      ++_playerShieldHitsTaken;
      int blockIndex = _shieldHealthBlocks.Length - _playerShieldHitsTaken;
      _shieldHealthBlocks[blockIndex].gameObject.SetActive(false);

      if (_playerShieldHitsTaken >= _shieldHealthBlocks.Length)
      {
        _shieldHealthDisplay.SetActive(false);
        _playerShieldActive = false;
      }
    }
    else
    {
      ++_playerHitsTaken;
      int blockIndex = _shipHealthBlocks.Length - _playerHitsTaken;
      _shipHealthBlocks[blockIndex].gameObject.SetActive(false);
    }
  }

  public void OnPlayerCollectedPowerUp(GameEventArgs args)
  {
    // shield
    // health
    // ammo
    return;
  }

  public void OnPlayerLivesChanged(GameEventArgs args)
  {
    _playerHitsTaken = 0;
    _livesImage.sprite = _livesSprites[args.iValue];

    if (args.iValue == 0)
    {
      _gameOverText.enabled = true;
      _restartInstructionText.enabled = true;
    }
  }
}
