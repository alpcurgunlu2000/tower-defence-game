using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager main;
    public Transform startPoint;
    public Transform[] path; // Enemy Path that the Enemy can take
    private void Awake() {
        main = this;
    }
}
