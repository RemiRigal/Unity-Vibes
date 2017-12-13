using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    private Dictionary<int, Object> objects;

    public enum TestEnum : int {
        First, Second
    }

    public class Test {
        public string test;
        public int test2;
        public TestEnum test3;
    }

	void Start () {
        string json = "{\"test\": \"Bonjour\", \"test2\": 12, \"test3\": 1}";
        Test o = JsonUtility.FromJson<Test>(json);
    }
}
