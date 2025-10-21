using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    public int bulletsFired;
    public int freezeHits;
    public int moveHits;
    public int npcsFrozen;
    public int respawnCount;
    public int npcDeaths;// name is off: now means: death caused by health = 0;
    public bool tutorialCompleted;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetData();
    }

    public void ResetData()
    {
        bulletsFired = 0;
        freezeHits = 0;
        moveHits = 0;
        npcsFrozen = 0;
        respawnCount = 0;
        npcDeaths = 0;
        tutorialCompleted = false;
    }


    public void RecordBulletFired()
    {
        bulletsFired++;
    }

    public void RecordFreezeHit()
    {
        freezeHits++;
    }

    public void RecordMoveHit()
    {
        moveHits++;
    }

    public void RecordNPCFrozen()
    {
        npcsFrozen++;
    }

    public void RecordRespawn()
    {
        respawnCount++;
    }

    public void RecordNPCDeath()
    {
        npcDeaths++;
    }

    public void CompleteTutorial()
    {
        tutorialCompleted = true;
    }
}
