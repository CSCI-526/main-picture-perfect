using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBallManager : MonoBehaviour
{
    public List<TargetBall> balls = new List<TargetBall>();// List of all TargetBall objects this manager controls
    public GameObject bridgePrefab;
    public Transform bridgeSpawnPoint;

    private int frozenCount = 0;

    void Awake()
    {
        // Auto-populate balls list if empty
        if (balls.Count == 0)
        {
            balls.AddRange(GetComponentsInChildren<TargetBall>());
        }

        foreach (var ball in balls)
        {
            ball.manager = this; // Set manager reference in each ball
        }
    }

    public void OnBallFrozen(TargetBall ball)
    {
        frozenCount++;
        Debug.Log($"Ball frozen: {ball.name}, total frozen = {frozenCount}");

        if (frozenCount == balls.Count)// All balls are frozen
        {
            Debug.Log("All balls frozen! Generating bridge...");
            if (bridgePrefab && bridgeSpawnPoint)// Spawn the bridge
            {
                Instantiate(bridgePrefab, bridgeSpawnPoint.position, bridgeSpawnPoint.rotation);
            }
        }
    }

    public void OnBallUnfrozen(TargetBall ball)
    {
        frozenCount--;
        Debug.Log($"Ball unfrozen: {ball.name}, total frozen = {frozenCount}");
    }
}
