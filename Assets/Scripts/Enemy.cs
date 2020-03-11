using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Enemy : MonoBehaviour
{
  [SerializeField]
  private Vector3 _direction = Vector3.down;
  [SerializeField]
  private float _linearSpeed = 5.0f;
  [SerializeField]
  private Vector3 _velocity;
  [SerializeField]
  private SpawnServer _spawnServer;

  private float _randomXValue()
  {
    return Random.Range(_xBoundNeg, _xBoundPos);
  }

  private Animator _anim;
  private BoxCollider2D _bcol;
  private Rigidbody2D _rig;
  private AudioSource _audioSrc;

  void Start()
  {
    _anim = GetComponent<Animator>();
    _bcol = GetComponent<BoxCollider2D>();
    _rig = GetComponent<Rigidbody2D>();
    _audioSrc = GetComponent<AudioSource>();

    _velocity = _direction * _linearSpeed;
    transform.position = new Vector3(_randomXValue(), _yBoundPos, 0);

    StartCoroutine(_shootAtPlayer());
  }

  void Update()
  {
    Move();
    CheckBoundaries();
  }

  void Move()
  {
    transform.Translate(_velocity * Time.deltaTime);
  }

  [SerializeField]
  private float _xBoundNeg = -11;
  [SerializeField]
  private float _xBoundPos = 11;
  [SerializeField]
  private float _yBoundNeg = -8.0f;
  [SerializeField]
  private float _yBoundPos = 8.0f;
  [SerializeField]
  private bool _loopActive = true; // basically whether we're attacking the player or not
  void CheckBoundaries()
  {
    if (transform.position.y < _yBoundNeg && _loopActive)
      transform.position = new Vector3(_randomXValue(), _yBoundPos, 0);
  }


  [SerializeField]
  private float _minShootTime = 1.0f;
  [SerializeField]
  private float _maxShootTime = 3.0f;
  [SerializeField]
  private Vector3 _laserOffset = new Vector3(0.0f,-0.525f,0.0f);
  IEnumerator _shootAtPlayer()
  {
    while (_loopActive)
    {
      yield return new WaitForSeconds(Random.Range(_minShootTime, _maxShootTime));
      // being killed while waiting for the next
      // laser is a race condition on _loopActive
      if (_loopActive)
      {
        _spawnServer.createEnemyLaser(transform.position + _laserOffset);
      }
    }
  }


  [SerializeField]
  private float _touchAttackStrength = 10.0f;
  [SerializeField]
  private int _pointValue = 10;
  [SerializeField]
  private GameEvent _deathEvent;
  [SerializeField]
  private GameEvent _killedByPlayerEvent;
  [SerializeField]
  private GameEvent _collidedWithPlayerEvent;

  private void OnTriggerEnter2D(Collider2D other)
  {
    GameEventArgs args;
    switch (other.tag)
    {
      case "Player":
        args = GameEventArgs.damageArgs(_touchAttackStrength, this.gameObject, other.gameObject);
        _collidedWithPlayerEvent.Raise(args);
        BeginDeathSequence();
        break;
      case "Laser":
        args = GameEventArgs.scoreArgs(_pointValue, this.gameObject, other.gameObject);
        _killedByPlayerEvent.Raise(args);
        Destroy(other.gameObject);
        BeginDeathSequence();
        break;
      default:
        // don't destroy enemy when you, for instance, hit a PowerUp
        break;
    }
  }

  [SerializeField]
  private string _enemyDestroyedTrigger = "OnEnemyDestroyed";

  private void BeginDeathSequence()
  {
    _bcol.enabled = false;
    _rig.simulated = false;
    _velocity /= 4; // don't just stop but intead slowly grind to a halt
    _loopActive = false;
    _anim.SetTrigger(_enemyDestroyedTrigger);
  }

  private void OnDeathAnimationBegin()
  {
    _audioSrc.Play(0);
  }

  private void OnDeathAnimationFinished()
  {
    Destroy(this.gameObject);
  }

  private void OnDestroy()
  {
    _deathEvent.Raise();
  }
}
