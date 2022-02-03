using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperGenerator : MonoBehaviour {

    [SerializeField] private GameObject baseSegment;
    [SerializeField] private GameObject midSegment;
    [SerializeField] private GameObject roofSegment;

    private bool generationComplete = false;
    
    void Start() {
        int segments = Random.Range(1,3);
        int height = Random.Range(30, 60);
        int segmentHeight = height / segments;
        float scale = Random.Range(0.85f, 1.2f);
        
        //Debug.Log("Generating skyscraper with height " + height + ", " + segments + " segments and a scale of " + scale);

        GameObject baseGO = Instantiate(baseSegment, transform);
        baseGO.transform.parent = transform;
        baseGO.transform.localScale = new Vector3(scale, 1.0f, scale);
        
        Vector3 pos = transform.position;

        int createdSegments = 0;

        for (int i = 0; i < segments; i++) {
            for (int j = 0; j < segmentHeight + (i == 0 ? height % segments : 0); j++) {
                GameObject go = Instantiate(midSegment, new Vector3(pos.x, pos.y + 8 + (createdSegments*4), pos.z), Quaternion.identity);
                go.transform.localScale = new Vector3(scale-(i*0.1f), 1, scale-(i*0.1f));
                go.transform.parent = transform;
                createdSegments++;
            }
        }

        GameObject roofGO = Instantiate(roofSegment, new Vector3(pos.x, pos.y + 8 + (createdSegments*4), pos.z), Quaternion.identity);
        roofGO.transform.parent = transform;
        roofGO.transform.localScale = new Vector3(scale-((segments-1)*0.1f), 1, scale-((segments-1)*0.1f));

        generationComplete = true;
    }

    public bool IsGenerationComplete() {
        return generationComplete;
    } 
}
