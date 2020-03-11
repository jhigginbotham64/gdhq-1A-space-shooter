using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { TRIPLE_SHOT = 0, SPEED_BOOST = 1, SHIELD = 2 }
public class PowerUp : MonoBehaviour
{
  [SerializeField]
  private Vector3 _direction = Vector3.down;
  [SerializeField]
  private float _linearSpeed = 10.0f;
  [SerializeField]
  private Vector3 _velocity;
  [SerializeField]
  private AudioClip _collectedSound;

  private float _randomXValue()
  {
    return Random.Range(_xBoundNeg, _xBoundPos);
  }

  void Start()
  {
    _velocity = _direction * _linearSpeed;
    transform.position = new Vector3(_randomXValue(), _yBoundPos, 0);
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
  void CheckBoundaries()
  {
    if (transform.position.y < _yBoundNeg) Destroy(this.gameObject);
  }

  [SerializeField]
  private PowerUpType _powerUpType = PowerUpType.TRIPLE_SHOT;
  public PowerUpType GetPowerUpType()
  {
    return _powerUpType;
  }

  [SerializeField]
  private GameEvent _collectedEvent;
  [SerializeField]
  private float _collectedSoundVolume = 2.0f;
  private void OnTriggerEnter2D(Collider2D other)
  {
    switch (other.tag)
    {
      case "Player":
        GameEventArgs args = GameEventArgs.interactArgs(this.gameObject, other.gameObject);
        _collectedEvent.Raise(args);
        AudioSource.PlayClipAtPoint(_collectedSound, transform.position, _collectedSoundVolume);
        Destroy(this.gameObject);
        break;
      default:
        break;
    }
  }

  [SerializeField]
  private GameEvent _destroyedEvent;
  private void OnDestroy()
  {
    _destroyedEvent.Raise();
  }
}
