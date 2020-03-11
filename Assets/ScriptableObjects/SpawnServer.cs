using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
  public string entityName;
  public GameObject prefab;
  public int spawnLimit;
  public Vector3[] spawnPositions;
  public Quaternion[] spawnRotations;

}

[CreateAssetMenu(fileName = "SpawnServer", menuName = "ScriptableObjects/SpawnServer", order = 1)]
public class SpawnServer : ScriptableObject
{
  [SerializeField]
  private List<SpawnData> entities;
  [SerializeField]
  private string _powerUpSuffix = "PowerUp";

  private bool haveEntity(string entityName)
  {
    return entities.Any((e) => e.entityName == entityName);
  }

  private SpawnData firstEntity(string entityName)
  {
    return entities.First((e) => e.entityName == entityName);
  }

  private List<SpawnData> powerUps()
  {
    return new List<SpawnData>(entities.Where((e) => e.entityName.EndsWith(_powerUpSuffix)));
  }

  public SpawnData getSpawnData(string entityName)
  {
    if (haveEntity(entityName))
    {
      return firstEntity(entityName);
    }
    else
    {
      return null;
    };
  }

  public GameObject createEntity(string entityName, Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    if (haveEntity(entityName) && firstEntity(entityName).prefab != null)
    {
      SpawnData ed = firstEntity(entityName);
      Vector3 startPosition = (atPosition != null) ? (Vector3)atPosition : (ed.spawnPositions.Length > 0) ? ed.spawnPositions[0] : Vector3.zero;
      Quaternion startRotation = (atRotation != null) ? (Quaternion)atRotation : (ed.spawnRotations.Length > 0) ? ed.spawnRotations[0] : Quaternion.identity;
      return Instantiate(ed.prefab, startPosition, startRotation);
    }
    else
    {
      return null;
    };
  }

  public GameObject createLaser(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("Laser", atPosition, atRotation);
  }

  public GameObject createTripleLaser(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("TripleLaser", atPosition, atRotation);
  }

  public GameObject createEnemy(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("Enemy", atPosition, atRotation);
  }

  public GameObject createShield(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("Shield", atPosition, atRotation);
  }

  public GameObject createPlayer(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("Player", atPosition, atRotation);
  }

  public GameObject createExplosion(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("Explosion", atPosition, atRotation);
  }

  public GameObject createAsteroid(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("Asteroid", atPosition, atRotation);
  }

  public GameObject createEnemyLaser(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    return createEntity("EnemyLaser", atPosition, atRotation);
  }

  public GameObject createRandomPowerUp(Vector3? atPosition = null, Quaternion? atRotation = null)
  {
    List<SpawnData> pups = powerUps();
    int pupCount = pups.Count;
    if (pupCount > 0)
    {
      int randomPupIndex = Random.Range(0, pupCount);
      return createEntity(pups[randomPupIndex].entityName, atPosition, atRotation);
    }
    else
    {
      return null;
    }
  }

}
