using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  [SerializeField]
  private Vector3 _startPosition = new Vector3(0, 0, 0);

  [SerializeField]
  private Vector3 _laserOffset = new Vector3(0, 1.05f, 0);

  [SerializeField]
  private float _maxHealth = 30.0f;
  [SerializeField]
  private float _currentHealth;
  private float _damageVizThreshold1;
  private float _damageVizThreshold2;
  [SerializeField]
  private GameObject _rightEngineFire;
  [SerializeField]
  private GameObject _leftEngineFire;

  private Rigidbody2D _rig;
  private BoxCollider2D _col;

  void Start()
  {
    transform.position = _startPosition;
    _currentHealth = _maxHealth;

    _damageVizThreshold1 = _maxHealth * 2/3;
    _damageVizThreshold2 = _maxHealth * 1/3;

    _rig = GetComponent<Rigidbody2D>();
    _col = GetComponent<BoxCollider2D>();
  }

  void Update()
  {
    Move();
    CheckBoundaries();
    Shoot();
  }

  [SerializeField]
  private float _speed = 10.0f;
  [SerializeField]
  private Vector3 _moveDir = new Vector3();
  [SerializeField]
  private bool _speedBoostActive = false;
  [SerializeField]
  private float _speedBoostMult = 2.0f;
  [SerializeField]
  private float _thrusterSpeedMult = 1.5f;

  void Move()
  {
    float _speedMult = 1 * ((_speedBoostActive) ? _speedBoostMult : 1) * ((Input.GetKey(KeyCode.LeftShift)) ? _thrusterSpeedMult : 1);

    _moveDir.x = Input.GetAxis("Horizontal");
    _moveDir.y = Input.GetAxis("Vertical");
    transform.Translate(_moveDir * _speed * _speedMult * Time.deltaTime);
  }

  private float _xBoundNeg = -11;
  [SerializeField]
  private float _xBoundPos = 11;
  [SerializeField]
  private float _yBoundNeg = -6.0f;
  [SerializeField]
  private float _yBoundPos = 0;
  void CheckBoundaries()
  {
    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _yBoundNeg, _yBoundPos));

    if (transform.position.x > _xBoundPos) transform.position = new Vector3(_xBoundNeg, transform.position.y, 0);
    else if (transform.position.x < _xBoundNeg) transform.position = new Vector3(_xBoundPos, transform.position.y, 0);
  }

  [SerializeField]
  private SpawnServer _spawnServer;
  [SerializeField]
  private float _nextShoot = -1f;
  [SerializeField]
  private float _fireRate = 0.08f;
  [SerializeField]
  private bool _tripleShotActive = false;
  void Shoot()
  {
    if (Input.GetKey(KeyCode.Space) && Time.time > _nextShoot)
    {
      _nextShoot = Time.time + _fireRate;

      if (_tripleShotActive)
      {
        _spawnServer.createTripleLaser(transform.position + _laserOffset);
      }
      else
      {
        _spawnServer.createLaser(transform.position + _laserOffset);
      }
    }
  }

  public void OnHitTaken(GameEventArgs args)
  {
    Damage(args.fValue);
  }

  private void Damage(float damageAmount)
  {
    if (_shielded)
    {
      DeactivateShield();
    }
    else
    {
      _currentHealth -= damageAmount;

      if (_currentHealth <= 0)
      {
        Die();
      }
      else if (_currentHealth <= _damageVizThreshold2)
      {
        _rightEngineFire.SetActive(true);
        _leftEngineFire.SetActive(true);
      }
      else if (_currentHealth <= _damageVizThreshold1)
      {
        int coinToss = Random.Range(0,2);
        switch (coinToss)
        {
          case 0:
            _rightEngineFire.SetActive(true);
            break;
          case 1:
            _leftEngineFire.SetActive(true);
            break;
          default:
            break;
        }
      }
    }
  }

  [SerializeField]
  private GameEvent _deathEvent;
  [SerializeField]
  private GameObject _deathSequence;
  private void Die()
  {
    _col.enabled = false;
    _rig.simulated = false;
    _spawnServer.createExplosion(transform.position, Quaternion.identity);
    _deathEvent.Raise();
    Destroy(this.gameObject, 0.25f);
  }

  public void OnPowerUpCollected(GameEventArgs args)
  {
    switch (args.sender.GetComponent<PowerUp>().GetPowerUpType())
    {
      case PowerUpType.TRIPLE_SHOT:
        CollectTripleShotPowerUp();
        break;
      case PowerUpType.SPEED_BOOST:
        CollectSpeedBoostPowerUp();
        break;
      case PowerUpType.SHIELD:
        CollectShieldPowerUp();
        break;
      default:
        break;
    }
  }

  [SerializeField]
  private float _tripleShotCoolDownTime = 10.0f;
  private Coroutine _tscdc; // triple shot cool down coroutine
  private void CollectTripleShotPowerUp()
  {
    _tripleShotActive = true;
    if (_tscdc != null) StopCoroutine(_tscdc);
    _tscdc = StartCoroutine(_tscdcf());
  }

  [SerializeField]
  private float _speedBoostCoolDownTime = 10.0f;
  private Coroutine _sbcdc; // speed boost cool down coroutine
  private void CollectSpeedBoostPowerUp()
  {
    _speedBoostActive = true;
    if (_sbcdc != null) StopCoroutine(_sbcdc);
    _sbcdc = StartCoroutine(_sbcdcf());
  }

  [SerializeField]
  private bool _shielded = false;
  private GameObject _shield;
  private void CollectShieldPowerUp()
  {
    if (_shield == null && !_shield)
    {
      GameObject shield = _spawnServer.createShield();
      if (shield != null)
      {
        shield.transform.position = transform.position;
        shield.transform.parent = transform;
        _shielded = true;
        _shield = shield;
      }
    }
  }

  public void DeactivateShield()
  {
    if (_shield != null) Destroy(_shield);
    _shielded = false;
  }

  private IEnumerator _tscdcf() // triple shot cool down coroutine function
  {
    yield return new WaitForSeconds(_tripleShotCoolDownTime);
    _tripleShotActive = false;
  }

  private IEnumerator _sbcdcf() // speed boost cool down coroutine function
  {
    yield return new WaitForSeconds(_speedBoostCoolDownTime);
    _speedBoostActive = false;
  }
}
