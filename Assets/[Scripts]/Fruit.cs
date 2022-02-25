using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] Vector2Int boundaries;
    [SerializeField] float rotSpeed = 45f;

    private float eulerY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        RandomLocation();
    }
    public void RandomLocation()
    {
        Vector2 tempBoundary = (Vector2)boundaries * GameManager.Instance.arenaScaleXZ;

        int xPos = (int)Random.Range(-tempBoundary.x, +tempBoundary.x);
        float yPos = 0.5f;
        int zPos = (int)Random.Range(-tempBoundary.y, +tempBoundary.y);

        transform.position = new Vector3(xPos, yPos, zPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Head"))
        {
            RandomLocation();
            GameManager.Instance.OnFruitCollected();
            AudioManager.Instance.PlayAudio(Audio.Pickup, false);
        }
    }

    private void Update()
    {
        eulerY += rotSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, eulerY, 0f);
    }
}
