using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class SendToGoogle : MonoBehaviour
{
    [SerializeField] private string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSf2bLEcDVkrVAzzfQAHfGgP1lozEyIbGguBX5n9Y25YHCT5uQ/formResponse";

    private long _sessionID;
    private int _bulletsFired;
    private int _freezeHits;
    private int _moveHits;
    private int _npcsFrozen;
    private int _respawnCount;
    private int _npcDeaths;
    private bool _tutorialCompleted;
    private float _sessionTime;
    private int _furthestCheckpoint; 
    private int _platformSpentMostTimeOn;
    private int _fallingDeath; 

    private void Awake()
    {
        _sessionID = DateTime.Now.Ticks;
    }

    public void Send()
    {
        _bulletsFired = AnalyticsManager.Instance.bulletsFired;
        _freezeHits = AnalyticsManager.Instance.freezeHits;
        _moveHits = AnalyticsManager.Instance.moveHits;
        _npcsFrozen = AnalyticsManager.Instance.npcsFrozen;
        _respawnCount = AnalyticsManager.Instance.respawnCount;
        _npcDeaths = AnalyticsManager.Instance.npcDeaths;
        _tutorialCompleted = AnalyticsManager.Instance.tutorialCompleted;
        _sessionTime = AnalyticsManager.Instance.GetSessionDuration();
        _furthestCheckpoint = AnalyticsManager.Instance.furthestCheckpoint; 
        _platformSpentMostTimeOn = AnalyticsManager.Instance.platformSpentMostTimeOn;
        _fallingDeath = AnalyticsManager.Instance.fallingDeath; 

        StartCoroutine(Post(
            _sessionID.ToString(),
            _bulletsFired.ToString(),
            _freezeHits.ToString(),
            _moveHits.ToString(),
            _npcsFrozen.ToString(),
            _respawnCount.ToString(),
            _npcDeaths.ToString(),
            _sessionTime.ToString("F2"),
            _furthestCheckpoint.ToString(), 
            _platformSpentMostTimeOn.ToString(),
            _fallingDeath.ToString(),
            _tutorialCompleted ? "Yes" : "No"
        ));
    }

    private IEnumerator Post(
        string sessionID, string bulletsFired, string freezeHits, string moveHits,
        string npcsFrozen, string respawnCount, string npcDeaths,
        string sessionTime, string tutorialCompleted, string furthestCheckpoint, 
        string platformSpentMostTimeOn, string fallingDeath)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.694186398", sessionID);
        form.AddField("entry.406484629", bulletsFired);
        form.AddField("entry.1262019424", freezeHits);
        form.AddField("entry.2069151188", moveHits);
        form.AddField("entry.117882450", npcsFrozen);
        form.AddField("entry.1506887316", respawnCount);
        form.AddField("entry.1189061886", npcDeaths);
        form.AddField("entry.1858668493", sessionTime);
        form.AddField("entry.751980840", tutorialCompleted);
        form.AddField("entry.223699066", furthestCheckpoint);
        form.AddField("entry.273042120", platformSpentMostTimeOn);
        form.AddField("entry.2098620278", fallingDeath);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log(www.error);
            else
                Debug.Log($"Data uploaded: sessionID={_sessionID}, Bullets={_bulletsFired}, Freeze Hits={_freezeHits}, NPC Freeze{_npcsFrozen}");
                Deub.Log($"Respawn Count={_respawnCount}, NPC Deaths={_npcDeaths}, Tutorial Completed={_tutorialCompleted}, Session Time={_sessionTime}, ");
                Deub.Log($"Furthest Checkpoint={_furthestCheckpoint}, Platform Spent Most Time On={_platformSpentMostTimeOn}, Falling Death={_fallingDeath}");
                Debug.Log("Form upload complete!");
        }
    }
}
