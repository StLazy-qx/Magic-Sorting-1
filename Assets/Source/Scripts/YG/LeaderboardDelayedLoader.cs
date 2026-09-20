using System.Collections;
using UnityEngine;
using YG; 

public class LeaderboardDelayedLoader : MonoBehaviour
{
    [SerializeField] private float _delay = 1f;
    [SerializeField] private LeaderboardYG _leaderboard;

    private WaitForSeconds _waiting;

    private void Awake()
    {
    	_waiting = new WaitForSeconds(_delay);
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedUpdate());
    }

    private void OnDisable()
    {
        StopCoroutine(DelayedUpdate());
    }

    private IEnumerator DelayedUpdate()
    {
        _leaderboard.gameObject.SetActive(false);

        yield return _waiting;

        _leaderboard.UpdateLB();
        _leaderboard.gameObject.SetActive(true);
    }
}