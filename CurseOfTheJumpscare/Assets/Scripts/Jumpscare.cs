using System;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    // the player the image is following
    private Player player = null;
    // the amount of time (in seconds) the jumpscare will exist
    public float scareTime = 5.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // list of both players
        var players = GameObject.FindGameObjectsWithTag("Player");
        // search for the first player that doesn't already have a jumpscare 
        for (int i=0, n = players.Length; i<n; i++)
        {
            Player script = players[i].GetComponent<Player>();
            if (!script.isJumpscared()) {
                script.makeScared(true);
                player = script; break;
            }
        }
        // jumpscare created with no target, get rid of it
        if (player == null) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // set position and rotation to cover the player's view
        transform.position = player.view.transform.position + (3f * player.Facing());
        transform.eulerAngles = player.view.transform.eulerAngles;

        // jumpscare is over
        if (scareTime <= 0.0f) {
            player.makeScared(false);
            Destroy(gameObject);
        } else scareTime -= Time.deltaTime;
    }
}
