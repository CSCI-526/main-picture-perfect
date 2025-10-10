using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Transform spikes;         
    public float delay = 1.5f;       // wait t before shake
    public float shakeTime = 0.5f;   // how long to shake
    public float shakeAmount = 0.1f; // shake parameter
    public float riseTime = 0.6f;    // how long spikes take to rise
    public float stayTime = 1.5f;    // how long spikes stay visible
    private bool used = false;
    private Vector3 startPos;
    private Vector3 spikeStartPos;
    private Vector3 spikeUpPos;

    void Start()
    {
        startPos = transform.localPosition;

        if (spikes != null)
        {
            spikeStartPos = spikes.localPosition;
            spikeUpPos = spikeStartPos + new Vector3(0f, 1f, 0f); // how high they rise
            spikes.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TrapRoutine(other));
            used = true;
        }
    }

    IEnumerator TrapRoutine(Collider player)
    {
        yield return new WaitForSeconds(delay);

        // shake platform
        float t = 0f;
        while (t < shakeTime)
        {
            transform.localPosition = startPos + Random.insideUnitSphere * shakeAmount;
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = startPos;

        // activate spikes and make them rise
        if (spikes != null)
        {
            spikes.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < riseTime)
            {
                spikes.localPosition = Vector3.Lerp(spikeStartPos, spikeUpPos, elapsed / riseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            spikes.localPosition = spikeUpPos;
        }

        // damage player
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null) hp.TakeDamage(1);

        // stay up for a while
        yield return new WaitForSeconds(stayTime);

        // hide spikes (go back down)
        if (spikes != null)
        {
            float elapsed = 0f;
            while (elapsed < riseTime)
            {
                spikes.localPosition = Vector3.Lerp(spikeUpPos, spikeStartPos, elapsed / riseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            spikes.localPosition = spikeStartPos;
            spikes.gameObject.SetActive(false);
        }

        // disable collider so it never triggers again
        GetComponent<Collider>().enabled = false;
    }
}
