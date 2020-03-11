using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// thank you to Ryan Hipple and Richard Fine:
// https://unity.com/how-to/architect-game-code-scriptable-objects
// https://www.youtube.com/watch?v=6vmRwLYWNRo
// https://www.youtube.com/watch?v=raQ3iHhE_Kk

/*
    something about this class that wasn't immediately clear to me
    until I started using it: having a public UnityEvent on a Monobehavior
    (which can be used as a Component) is a very powerful pattern,
    you can have one GameEventListener that potentially invokes
    multiple different functions on multiple different objects in
    response to the same event.

    SPOILER: i actually did this, and additionally put a bunch of
    GameEventListeners on a single prefab to keep track of all
    responses to all events per scene.

    GameEventArgs is my own addition, no idea how "good" it is.
*/

[System.Serializable]
public class GameEventArgs
{
  public int iValue = 0;
  public float fValue = 0.0f;
  public GameObject sender;
  public GameObject target;

  public static GameEventArgs iValueWithSender(int _iValue, GameObject _sender)
  {
    GameEventArgs args = new GameEventArgs();
    args.iValue = _iValue;
    args.sender = _sender;
    return args;
  }

  public static GameEventArgs iValueWithSenderAndTarget(int _iValue, GameObject _sender, GameObject _target)
  {
    GameEventArgs args = new GameEventArgs();
    args.iValue = _iValue;
    args.sender = _sender;
    args.target = _target;
    return args;
  }

  public static GameEventArgs fValueWithSenderAndTarget(float _fValue, GameObject _sender, GameObject _target)
  {
    GameEventArgs args = new GameEventArgs();
    args.fValue = _fValue;
    args.sender = _sender;
    args.target = _target;
    return args;
  }

  public static GameEventArgs SenderAndTarget(GameObject _sender, GameObject _target)
  {
    GameEventArgs args = new GameEventArgs();
    args.sender = _sender;
    args.target = _target;
    return args;
  }

  public static GameEventArgs scoreArgs(int _iValue, GameObject _sender, GameObject _target)
  {
    return iValueWithSenderAndTarget(_iValue, _sender, _target);
  }

  public static GameEventArgs livesChangedArgs(int _iValue, GameObject _sender)
  {
    return iValueWithSender(_iValue, _sender);
  }

  public static GameEventArgs damageArgs(float _fValue, GameObject _sender, GameObject _target)
  {
    return fValueWithSenderAndTarget(_fValue, _sender, _target);
  }

  public static GameEventArgs interactArgs(GameObject _sender, GameObject _target)
  {
    return SenderAndTarget(_sender, _target);
  }
}

[System.Serializable]
public class UnityEventWithArgs : UnityEvent<GameEventArgs>
{ }

public class GameEventListener : MonoBehaviour
{
  public GameEvent Event;
  public UnityEventWithArgs Response;

  private void OnEnable()
  {
    Event.RegisterListener(this);
  }

  private void OnDisable()
  {
    Event.UnregisterListener(this);
  }

  public void OnEventRaised(GameEventArgs args = null)
  {
    Response.Invoke(args);
  }
}
