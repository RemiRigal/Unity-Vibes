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

    public AnimationFrame GetFrame(float time) {
        if (time < animationStart || time > animationStart + animationDuration) {
            return null;
        }
        int frameIndex = (int) Mathf.Floor((time - animationStart) / deltaTime);
        float ratio = ((time - animationStart) % deltaTime) / deltaTime;
        Vector3 position = Vector3.Lerp(positions[frameIndex], positions[frameIndex+1], ratio);
        Quaternion rotation = Quaternion.Euler(Vector3.Lerp(rotations[frameIndex], rotations[frameIndex + 1], ratio));
        return new AnimationFrame(position, rotation);
    }

    public class AnimationFrame {
        public Vector3 position;
        public Quaternion rotation;
        public AnimationFrame(Vector3 position, Quaternion rotation) {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
