using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D _rig;
    private CircleCollider2D _col;

    [SerializeField]
    private SpawnServer _spawnServer;

    void Start()
    {
      _rig = GetComponent<Rigidbody2D>();
      _col = GetComponent<CircleCollider2D>();
    }

    [SerializeField]
    private GameEvent _destroyedByPlayerEvent; // laser

    private void OnTriggerEnter2D(Collider2D other)
    {
      switch (other.tag)
      {
        case "Laser":
          Destroy(other.gameObject);
          _destroyedByPlayerEvent.Raise();
          BeginDeathSequence();
          break;
        default:
          // only care about lasers
          break;
      }
    }

    void BeginDeathSequence()
    {
      _col.enabled = false;
      _rig.simulated = false;
      _spawnServer.createExplosion(transform.position, Quaternion.identity);
      Destroy(this.gameObject, 0.25f);
    }
}
