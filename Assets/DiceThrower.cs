﻿﻿using System.Collections.Generic;
using UnityEngine;

public class DiceThrower : MonoBehaviour
{
    public Dice dicePrefab;
    public int amountOfDice = 5;
    public float throwForce = 5f;
    public float rollForce = 5f;
    public List<Dice> spawnedDice = new List<Dice>();

    private int rollCount = 0;
    private bool isWorthyOfRoll = true;
    public BoxCollider spawnArea;

    public RoundManager roundManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isWorthyOfRoll)
        {
            RollDice();
        }
    }

    private void SpawnDice()
    {
        foreach (var die in spawnedDice)
        {
            Destroy(die.gameObject);
        }
        spawnedDice.Clear();

        for (int i = 0; i < amountOfDice; i++)
        {
            Vector3 center = spawnArea.transform.TransformPoint(spawnArea.center);
            Vector3 size = Vector3.Scale(spawnArea.size, spawnArea.transform.lossyScale);

            float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float y = center.y + 2f;
            float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
            Vector3 spawnPos = new Vector3(x, y, z);

            Dice newDice = Instantiate(dicePrefab, spawnPos, Quaternion.identity);
            spawnedDice.Add(newDice);
        }
    }

    private void RollDice()
    {

        if (!roundManager.roundActive)
        {
            roundManager.StartRound();
        }

        if (rollCount >= 3)
        {
            isWorthyOfRoll = false;
            return;
        }

        if (spawnedDice.Count == 0)
        {
            SpawnDice();
        }
        else
        {
            List<Dice> newSpawnList = new List<Dice>();

            for (int i = 0; i < spawnedDice.Count; i++)
            {
                Dice die = spawnedDice[i];

                if (die != null && die.isHeld)
                {
                    newSpawnList.Add(die);
                }
                else
                {
                    if (die != null)
                        Destroy(die.gameObject);

                    Vector3 center = spawnArea.transform.TransformPoint(spawnArea.center);
                    Vector3 size = Vector3.Scale(spawnArea.size, spawnArea.transform.lossyScale);

                    float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
                    float y = center.y + 2f;
                    float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
                    Vector3 spawnPos = new Vector3(x, y, z);

                    Dice newDie = Instantiate(dicePrefab, spawnPos, Quaternion.identity);
                    newSpawnList.Add(newDie);
                }
            }

            spawnedDice = newSpawnList;
        }

        rollCount++;

          roundManager?.RegisterRoll();
        StartCoroutine(ApplyForcesDelayed());
    }

    private System.Collections.IEnumerator ApplyForcesDelayed()
    {
        yield return new WaitForFixedUpdate();
        ApplyForces();
    }

    private void ApplyForces()
    {
        foreach (var die in spawnedDice)
        {
            if (die != null && !die.isHeld)
            {
                Rigidbody rb = die.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                Vector3 randomDirection = new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-6f, -2f),
                    Random.Range(-2f, 2f)
                );

                Vector3 randomTorque = new Vector3(
                    Random.Range(-7f, 7f),
                    Random.Range(-7f, 7f),
                    Random.Range(-7f, 7f)
                );

                rb.AddForce(randomDirection * throwForce, ForceMode.Impulse);
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
    }

    public void ResetDiceAndTimer()
    {
        foreach (var die in spawnedDice)
        {
            Destroy(die.gameObject);
        }
        spawnedDice.Clear();

        rollCount = 0;
        isWorthyOfRoll = true;
    }
}
