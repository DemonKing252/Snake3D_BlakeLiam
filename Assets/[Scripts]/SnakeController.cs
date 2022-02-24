using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SnakeController : MonoBehaviour
{
    private Vector3 direction = Vector3.right;
    private uint frameTick = 0;

    [SerializeField] private uint frameTickRate = 5;
    [SerializeField] private List<Transform> snakeElements;
    [SerializeField] private GameObject snakeBodyPrefab;

    [SerializeField] private float timeStep = 1f;
    [SerializeField] private float growRate = 5f;
    [SerializeField] private int startingSize = 3;

    private void Awake()
    {
        GameManager.Instance.onFruitCollected += OnFruitCollected;
    }
    private void OnDestroy()
    {
        GameManager.Instance.onFruitCollected -= OnFruitCollected;
    }



    // Start is called before the first frame update
    void Start()
    {
        snakeElements.Add(transform);
        // We already have the head, so subtract 1 to visually have the same amount of elements
        for (int i = 0; i < startingSize - 1; i++)
        {
            GrowSnake();
        }
        ResetSnake();
        //InvokeRepeating(nameof(GrowSnake), growRate, growRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnFruitCollected()
    {
        GrowSnake();
    }

    private void FixedUpdate()
    {
        frameTick++;
        if (frameTick % frameTickRate == 0)
            OnSnakeTick();
    }
    private void OnSnakeTick()
    {

        for (int i = snakeElements.Count - 1; i > 0; i--)
        {
            snakeElements[i].position = snakeElements[i - 1].position;
        }
        transform.position += direction * timeStep;

    }
    public void GrowSnake()
    {
        GameObject tail = Instantiate(snakeBodyPrefab);
        tail.transform.position = snakeElements[snakeElements.Count - 1].position;

        snakeElements.Add(tail.transform);
    }
    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("[BOUNDARY] Round loss...");
            OnRoundLoss();
        }
        else if (other.gameObject.CompareTag("Player"))
        {

            Debug.Log("[PLAYER] Round loss...");
            OnRoundLoss();
        }

    }
    public void ResetSnake()
    {
        // If there is a text animation going on, stop it
        GameManager.Instance.StopAllCoroutines();

        GameManager.Instance.ResetScore();
        GameManager.Instance.ResetLives();

        transform.position = new Vector3(0f, 0.5f, 0f);
        // We don't want to destroy the head of the snake
        // Start at index 1
        for (int i = 1; i < snakeElements.Count; i++)
        {
            Destroy(snakeElements[i].gameObject);
        }
        snakeElements.Clear();
        snakeElements.Add(transform);

        // We already have the head, so subtract 1 to visually have the same amount of elements
        for (int i = 0; i < startingSize - 1; i++)
        {
            GrowSnake();
        }

    }
    public void OnRoundLoss()
    {
        transform.position = new Vector3(0f, 0.5f, 0f);
        // If there is a text animation going on, stop it
        GameManager.Instance.StopAllCoroutines();

        GameManager.Instance.LoseLife();

        // We don't want to destroy the head of the snake
        // Start at index 1
        for (int i = 1; i < snakeElements.Count; i++)
        {
            Destroy(snakeElements[i].gameObject);
        }
        snakeElements.Clear();
        snakeElements.Add(transform);

        // We already have the head, so subtract 1 to visually have the same amount of elements
        for (int i = 0; i < startingSize - 1; i++)
        {
            GrowSnake();
        }

    }

    #region Input
    public void OnHorizontalAxisLeft(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            direction = Vector3.left;
        }
    }
    public void OnHorizontalAxisRight(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            direction = Vector3.right;
        }
    }
    public void OnVerticalAxisUp(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            direction = Vector3.forward;
        }
    }
    public void OnVerticalAxisDown(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            direction = Vector3.back;
        }
    }
#endregion
}
