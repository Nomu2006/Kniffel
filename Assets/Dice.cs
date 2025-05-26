using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Dice : MonoBehaviour
{
    public Transform[] diceFaces;
    public int diceIndex = -1;
    private bool hasStoppedRolling;
    private float stopDuration = 1f; 
    private float stopTimeCounter = 0f;
    public bool isHeld = false;
    public Rigidbody rb;
    private Collider col;
    public Renderer rend;
    public Color originalColor;
    public float stopThreshold = 0.2f;
    public DiceThrower diceThrower;
    public bool canBeLocked = false;


    public static UnityAction<int, int> OnDiceResult;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            rend.material = new Material(rend.material);
            originalColor = rend.material.color;
        }
    }

    private void Update()
    {
        if (!hasStoppedRolling)
        {
            if (rb.linearVelocity.sqrMagnitude < stopThreshold)
            {
                stopTimeCounter += Time.deltaTime;

                if (stopTimeCounter >= stopDuration)
                {
                canBeLocked = true;
                    if (isHeld)
                    {
                        stopTimeCounter = 0f;
                        hasStoppedRolling = true;
                    }
                }
            }
            else
            {
                stopTimeCounter = 0f;
            }
        }
    }

    [ContextMenu("Get Top Face")]
    public int GetNumberOnTopFace()
    {
        if (diceFaces == null || diceFaces.Length == 0) return -1;

        var topFace = 0;
        var lastYPosition = diceFaces[0].position.y;

        for (int i = 1; i < diceFaces.Length; i++)
        {
            if (diceFaces[i].position.y > lastYPosition)
            {
                lastYPosition = diceFaces[i].position.y;
                topFace = i;
            }
        }

        OnDiceResult?.Invoke(diceIndex, topFace + 1);
        return topFace + 1;
    }


    private void OnMouseDown()
    {
        if (canBeLocked == true)
        {
            isHeld = !isHeld;

            if (rend != null)
                rend.material.color = isHeld ? Color.green : originalColor;

            //if (col != null)
            //col.isTrigger = isHeld;

            if (rb != null)
            {
                rb.useGravity = !isHeld;
                rb.isKinematic = isHeld;
            }
        }
    }
}
