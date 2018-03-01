using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnimation {

    public List<Vector3> positions;
    public List<Vector3> rotations;

    public float deltaTime = 0f;
    public float animationDuration = 0f;

    public bool animate = false;
    public float animationStart = 0f;
    public float animationTime = 0f;

    public ObjectAnimation(float deltaTime, List<Vector3> positions, List<Vector3> rotations) {
        this.deltaTime = deltaTime;
        this.positions = positions;
        this.rotations = rotations;

        animationDuration = positions.Count * deltaTime;
    }

    public void Start() {
        animationTime = 0f;
        animate = true;
    }

    public AnimationFrame GetFrame(float dt) {
        animationTime += dt;
        int frameIndex = (int) Mathf.Floor((animationTime - animationStart) / deltaTime);
        if (frameIndex + 1 >= positions.Count || animationTime > animationStart + animationDuration) {
            animate = false;
            return new AnimationFrame(positions[positions.Count - 1], Quaternion.Euler(rotations[rotations.Count - 1]));
        }
        float ratio = ((animationTime - animationStart) % deltaTime) / deltaTime;
        Vector3 position = Vector3.Lerp(positions[frameIndex], positions[frameIndex+1], ratio);
        Quaternion rotation = Quaternion.Euler(Vector3.Lerp(rotations[frameIndex], rotations[frameIndex + 1], ratio));
        return new AnimationFrame(position, rotation);
    }
}

public class AnimationFrame {

    public Vector3 position;
    public Quaternion rotation;

    public AnimationFrame(Vector3 position, Quaternion rotation) {
        this.position = position;
        this.rotation = rotation;
    }
}
