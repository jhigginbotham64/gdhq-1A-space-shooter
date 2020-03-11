using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// thanks again to Ryan Hipple and Richard Fine:
// https://unity.com/how-to/architect-game-code-scriptable-objects
// https://www.youtube.com/watch?v=6vmRwLYWNRo
// https://www.youtube.com/watch?v=raQ3iHhE_Kk

[CreateAssetMenu(fileName = "GameEvent", menuName = "ScriptableObjects/GameEvent", order = 1)]
public class GameEvent : ScriptableObject
{
  private List<GameEventListener> listeners =
  new List<GameEventListener>();

  public void Raise(GameEventArgs args = null)
  {
    for (int i = listeners.Count - 1; i >= 0; i--)
      listeners[i].OnEventRaised(args);
  }

  public void RegisterListener(GameEventListener listener)
  {
    listeners.Add(listener);
  }

  public void UnregisterListener(GameEventListener listener)
  {
    listeners.Remove(listener);
  }
}
