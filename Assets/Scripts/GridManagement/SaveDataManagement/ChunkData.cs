using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GridManagement {
    public class ChunkData {
        //METADATA
        private string gameVersion = "0.1";
        
        
        public int testId = 0;
        public GameObject[,] chunkGrid = null;

        public ChunkData(int testId, GameObject[,] chunkGrid) {
            this.testId = testId;
            this.chunkGrid = chunkGrid;
        }
    }
}