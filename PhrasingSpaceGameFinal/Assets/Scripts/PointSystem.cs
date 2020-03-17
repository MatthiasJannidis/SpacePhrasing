using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    enum PlayerState 
    {
        Safe,
        Dangerous,
        ExitingDangerous,
        Dead
    }
    PlayerState state = PlayerState.Safe;

    RaycastHit2D[] hits = new RaycastHit2D[10];

    Rigidbody2D playerRigidbody;
    Timer safeTimer;
    int score = 0;
    int tempScore = 0;
    int maxScore;
    [SerializeField] GameSettingsSO settings;
    [SerializeField] UnityEngine.UI.Text scoreText;
    [SerializeField] UnityEngine.UI.Text tempScoreText;
    [SerializeField] UnityEngine.UI.Text maxScoreText;

    void Start()
    {
        var player = FindObjectOfType<DeathSystem>();
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        player.onDie.AddListener(onDeath);
        player.onRevive.AddListener(onRevive);
        maxScore = PlayerPrefs.GetInt("_score", 0);
        safeTimer = new Timer(.0f);

        scoreText.text = score.ToString();
        tempScoreText.text = tempScore.ToString();
        //maxScoreText.text = maxScore.ToString();
    }

    void addScore(int newScore) 
    {
        score += newScore;
        scoreText.text = score.ToString();
        if(score > maxScore) 
        {
            PlayerPrefs.SetInt("_score", score);
            maxScore = score;
            //maxScoreText.text = maxScore.ToString();
        }
    }

    void onDeath() 
    {
        tempScore = 0;
        tempScoreText.text = tempScore.ToString();
        state = PlayerState.Dead;
    }

    void onRevive() 
    {
        state = PlayerState.Safe;
    }

    // Update is called once per frame
    void Update()
    {
        int hitCount = Physics2D.CircleCastNonAlloc(playerRigidbody.transform.position, settings.pointRadius, Vector2.one, hits, .0f);
        bool hit = hitCount > 1; // 1 : player
        
        switch (state) 
        {
            case PlayerState.Safe:
                if (hit) 
                {
                    state = PlayerState.Dangerous;
                }
                break;
            case PlayerState.Dangerous:
                if (hit) 
                {
                    for(int i = 0; i < hitCount; i++) 
                    {
                        var planet = hits[i].transform.gameObject.GetComponent<GravityEffector>();
                        if (planet == null) continue;
                        float score = Vector2.Distance(playerRigidbody.transform.position.xy(), hits[i].transform.position.xy());
                        score -= planet.Radius;
                        tempScore += Mathf.CeilToInt(settings.scoreMultiplier * playerRigidbody.velocity.magnitude * score);
                    }

                    tempScoreText.text = tempScore.ToString();

                } else 
                {
                    safeTimer.reset(settings.safeTime);
                    state = PlayerState.ExitingDangerous;
                }
                break;
            case PlayerState.ExitingDangerous:
                safeTimer.tick();
                if (safeTimer.isDone) 
                {
                    addScore(tempScore);
                    tempScore = 0;
                    tempScoreText.text =  tempScore.ToString();
                    state = PlayerState.Safe;
                }
                break;
        }


    }
}
