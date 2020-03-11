using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
  private AudioSource _audioSrc;
  void Start()
  {
    _audioSrc = GetComponent<AudioSource>();
  }
  void OnExplosionAnimationFinished()
  {
    Destroy(this.gameObject);
  }
  void OnExplosionAnimationStart()
  {
    _audioSrc.Play(0);
  }
}
