using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountTrees : MonoBehaviour
{
    private int count;
    private int active = 0;

    // Update is called once per frame
    void Update()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        count = active;
        active = 0;
        for (int i = 0; i < trees.Length; i++)
            if (trees[i].transform.Find("kmen").gameObject.activeSelf)
                active++;
        
        if (count != active)
            Debug.Log("active trees: " + active);
    }
}
