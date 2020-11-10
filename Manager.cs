using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Manager does not exist");
            }
            return _instance;
        }
    }

    public Text ballCountText;
    public int maxBallsJoinedThreshold = 50;
    public event System.Action OnDoneSpawning;

    private int _ballCount;
    private float _splitForce = 40f;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        StartCoroutine(StartSpawningBalls(0.25f));
    }

    private IEnumerator StartSpawningBalls(float delay)
    {
        while (_ballCount < PoolManager.Instance.maxBallNumber)
        {
            float randomWidth = Random.Range(0, Screen.width);
            float randomHeight = Random.Range(0, Screen.height);
            float randomDepth = Random.Range(Camera.main.nearClipPlane+10, Camera.main.farClipPlane/15); //values for not to close and not to far from camera view
            Vector3 _screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(randomWidth, randomHeight, randomDepth));

            GameObject newBall = PoolManager.Instance.RequestBall();
            newBall.transform.position = _screenPosition;
            newBall.transform.parent = this.transform;

            _ballCount++;
            ballCountText.text = "Ball count: " + _ballCount;

            yield return new WaitForSeconds(delay);
        }
        OnDoneSpawning?.Invoke();
        yield break;
    }

    public void SplitBall(Vector3 splitPosition, int numberOfBalls)
    {
        for (int i = 0; i < numberOfBalls; i++)
        {
            GameObject newBall = PoolManager.Instance.RequestBall();
            newBall.transform.position = splitPosition;
            newBall.transform.parent = this.transform;
            StartCoroutine(DeactivateCollider(newBall, 0.5f));

            float randomSplitForceX = Random.Range(-_splitForce, _splitForce);
            float randomSplitForceY = Random.Range(-_splitForce, _splitForce);
            float randomSplitForceZ = Random.Range(-_splitForce, _splitForce);
            Vector3 randomVector = new Vector3(randomSplitForceX, randomSplitForceY, randomSplitForceZ);
            newBall.GetComponent<Rigidbody>().AddForce(randomVector, ForceMode.Impulse);
        }
    }

    private IEnumerator DeactivateCollider(GameObject ball, float deactivationTime)
    {
        SphereCollider ballCollider = ball.GetComponent<SphereCollider>();
        GravityForce ballGravityForce = ball.GetComponent<GravityForce>();

        ballCollider.enabled = false;
        ballGravityForce.enabled = false;
        yield return new WaitForSeconds(deactivationTime);
        ballCollider.enabled = true;
        ballGravityForce.enabled = true;
        yield break;
    }


}
