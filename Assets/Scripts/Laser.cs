using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
  [SerializeField]
  private bool _isEnemyLaser = false;
  [SerializeField]
  private float _laserDamage = 10.0f;
  [SerializeField]
  private Vector3 _direction = Vector3.up;
  [SerializeField]
  private float _linearSpeed = 32.0f;
  [SerializeField]
  private AudioClip _shootClip;
  private Rigidbody2D _rig;
  private BoxCollider2D _col;
  private SpriteRenderer _sr;
  [SerializeField]
  private float _laserSoundVolume = 1.0f;
  void Start()
  {
    _rig = GetComponent<Rigidbody2D>();
    _col = GetComponent<BoxCollider2D>();
    _sr = GetComponent<SpriteRenderer>();

    AudioSource.PlayClipAtPoint(_shootClip, transform.position, _laserSoundVolume);
  }

  void Update()
  {
    Move();
    CheckBoundaries();
  }

  void Move()
  {
    transform.Translate(_direction * _linearSpeed * Time.deltaTime);
  }

  [SerializeField]
  private float _yBoundPos = 8.0f;
  [SerializeField]
  private float _yBoundNeg = -8.0f;
  void CheckBoundaries()
  {
    if (transform.position.y > _yBoundPos || transform.position.y < _yBoundNeg)
    {
      Destroy(this.gameObject);
    }
  }

  [SerializeField]
  private GameEvent _collidedWithPlayerEvent;

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (_isEnemyLaser)
    {
      GameEventArgs args;
      switch (other.tag)
      {
        case "Player":
          args = GameEventArgs.damageArgs(_laserDamage, this.gameObject, other.gameObject);
          _collidedWithPlayerEvent.Raise(args);
          Destroy(this.gameObject);
          break;
        default:
          break;
      }
    }
  }

  // if all lasers (in a TripleLaser or EnemyLaser collide and
  // are destroyed before the boundary is reached, then the parent
  // will remain indefinitely. this causes the last laser destroyed
  // to clean up the parent.
  void OnDestroy()
  {
    if (transform.parent != null && transform.parent.childCount <= 1)
    {
      Destroy(transform.parent.gameObject);
    }
  }
}
