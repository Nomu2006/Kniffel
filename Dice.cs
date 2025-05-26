using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]

public class Dice : MonoBehaviour
{
    public Transform[] diceFaces;
    public Rigidbody rb;

    private int _diceIndex = -1;

    private bool _hasStoppedRolling;
    private bool _delayFinished;

    public static UnityAction<int, int> OnDiceResult;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_delayFinished) return;

        if (!_hasStoppedRolling && rb.linearVelocity.sqrMagnitude == 0f)
        {
            _hasStoppedRolling = true;
            GetNumberOnTopFace();
        }
    }
    [ContextMenu("Get Top Face")]
    private int GetNumberOnTopFace()
    {
        if (diceFaces == null) return -1;

        var topFace = 0;
        var lastYPosition = diceFaces[0].position.y;

        for (int i = 0; i < diceFaces.Length; i++)
        {
            if (diceFaces[i].position.y > lastYPosition)
            {
                lastYPosition = diceFaces[i].position.y;
                topFace = i;
            }
        }

        Debug.Log($"Dice result {topFace + 1}");

        OnDiceResult?.Invoke(_diceIndex, topFace + 1);
        return topFace + 1;

    }
    public void RollDice(float throwForce, float rollForce, int i)
    {
        _diceIndex = i;
        var randomVarience = UnityEngine.Random.Range(-1f, 1f);
        rb.AddForce(transform.forward * (throwForce + randomVarience), ForceMode.Impulse);

        var randX = UnityEngine.Random.Range(0f, 1f);
        var randY = UnityEngine.Random.Range(0f, 1f);
        var randZ = UnityEngine.Random.Range(0f, 1f);

        rb.AddTorque(new Vector3(randX, randY, randZ) * (rollForce + randomVarience), ForceMode.Impulse);

        DelayResult();
    }

    private async void DelayResult()
    {
        await Task.Delay(1000);
        _delayFinished = true;
    }
}
