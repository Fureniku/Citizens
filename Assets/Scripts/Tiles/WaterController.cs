using System.Collections;
using System.Collections.Generic;
using Loading.States;
using UnityEngine;

public class WaterController : MonoBehaviour {

    public void Initialize() {
        float worldSize = World.Instance.GetChunkManager().GetSize() * Chunk.size;
        float worldScale = World.Instance.GetChunkManager().GetGridTileSize();
        Debug.LogError("Setting water size to " + World.Instance.GetChunkManager().GetSize() + " * " + Chunk.size);

        transform.position = new Vector3(worldSize / 2.0f * worldScale, transform.position.y, worldSize / 2.0f * worldScale);
        transform.localScale = new Vector3(worldSize * 2, 1.0f, worldSize * 2);
    }
}
