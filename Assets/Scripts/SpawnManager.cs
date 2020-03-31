using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
  [SerializeField]
  private SpawnServer _spawnServer;
  [SerializeField]
  private int _playerLivesRemaining = 3;
  [SerializeField]
  private GameEvent _playerSpawnedEvent;
  [SerializeField]
  private bool _spawning = true;
  [SerializeField]
  private GameObject _enemyContainer;
  [SerializeField]
  private GameObject _powerUpContainer;
  [SerializeField]
  private KeyCode _spawnKey = KeyCode.Return;
  [SerializeField]
  private bool _sadistic = false;
  [SerializeField]
  private float _enemySpawnFrequency = 5.0f;

  [SerializeField]
  private int _maxEnemies = 30;

  [SerializeField]
  private int _currentEnemyCount;
  [SerializeField]
  private int _maxPowerUps = 3;
  [SerializeField]
  private int _currentPowerUpCount;
  [SerializeField]
  private float _startUpDelay = 3.0f;

  private Coroutine _enemySpawnRoutine;
  private Coroutine _powerUpSpawnRoutine;

  void Start()
  {
    _currentEnemyCount = 0;
    _currentPowerUpCount = 0;

    _spawnPlayerOne();
  }

  public void OnAsteroidDestroyed()
  {
    _enemySpawnRoutine = StartCoroutine(_spawnEnemies());
    _powerUpSpawnRoutine = StartCoroutine(_spawnPowerUps());
  }

  void Update()
  {
    if ((_sadistic && Input.GetKey(_spawnKey)) || Input.GetKeyDown(_spawnKey)) _spawnEnemy();
  }

  void _spawnPlayerOne()
  {
    _spawnServer.createPlayer();
    _playerSpawnedEvent.Raise();
  }

  IEnumerator _spawnPlayerOneDelayed()
  {
    yield return new WaitForSeconds(_startUpDelay);
    _spawnPlayerOne();
  }

  void _spawnEnemy()
  {
    if (_sadistic || _currentEnemyCount < _maxEnemies)
    {
      GameObject e = _spawnServer.createEnemy();
      e.transform.parent = _enemyContainer.transform;
      _currentEnemyCount++;
    }
  }

  IEnumerator _spawnEnemies()
  {
    yield return new WaitForSeconds(_startUpDelay);

    while (_spawning)
    {
      _spawnEnemy();
      yield return new WaitForSeconds(_enemySpawnFrequency);
    }
  }

  void _spawnPowerUp()
  {
    if (_currentPowerUpCount < _maxPowerUps)
    {
      GameObject p = _spawnServer.createRandomPowerUp();
      if (p != null)
      {
        p.transform.parent = _powerUpContainer.transform;
        _currentPowerUpCount++;
      }
    }
  }

  [SerializeField]
  private float _minPowerUpTime = 3.0f;
  [SerializeField]
  private float _maxPowerUpTime = 7.0f;
  IEnumerator _spawnPowerUps()
  {
    yield return new WaitForSeconds(_startUpDelay);

    while (_spawning)
    {
      yield return new WaitForSeconds(Random.Range(_minPowerUpTime, _maxPowerUpTime));
      _spawnPowerUp();
    }
  }

  [SerializeField]
  private GameEvent _playerLivesChangedEvent;
  public void OnPlayerDeath()
  {
    --_playerLivesRemaining;
    if (_playerLivesRemaining > 0)
    {
      StartCoroutine(_spawnPlayerOneDelayed());
    }
    else
    {
      _spawning = false;
      // "game over"
    }

    GameEventArgs args = GameEventArgs.livesChangedArgs(_playerLivesRemaining, this.gameObject);
    _playerLivesChangedEvent.Raise(args);
  }

  public void OnEnemyDestroyed()
  {
    _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
  }

  public void OnPowerUpDestroyed()
  {
    _currentPowerUpCount = Mathf.Max(0, _currentPowerUpCount - 1);
  }
}
