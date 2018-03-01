using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {

    public static AnimationManager Instance = null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void AddAnimation(MainObject obj, ObjectAnimation animation) {
        obj.SetAnimation(animation);
    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
