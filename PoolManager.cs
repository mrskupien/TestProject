using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("PoolManager does not exist");
            }
            return _instance;
        }
    }

    public GameObject ballPrefab;
    private List<GameObject> _ballPoolList = new List<GameObject>();

    public int maxBallNumber = 250;

    private void Awake()
    {
        _instance = this;
        GenerateBalls(maxBallNumber);
    }

    private List<GameObject> GenerateBalls(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newBall = Instantiate(ballPrefab);
            newBall.transform.parent = this.transform;
            newBall.SetActive(false);
            _ballPoolList.Add(newBall);
        }
        return _ballPoolList;
    }

    public GameObject RequestBall()
    {
        foreach (var ball in _ballPoolList)
        {
            if (ball.activeInHierarchy == false)
            {
                ball.SetActive(true);
                return ball;
            }
        }
        GameObject newBall = Instantiate(ballPrefab);
        newBall.transform.parent = this.transform;
        _ballPoolList.Add(newBall);
        return newBall;
    }

}
