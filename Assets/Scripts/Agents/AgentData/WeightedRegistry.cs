using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WeightedRegistry {

    private List<WeightedOption> list = new List<WeightedOption>();

    public void AddToList(WeightedOption wo) {
        list.Add(wo);
    }

    private float ListValue() {
        float value = 0;
        for (int i = 0; i < list.Count; i++) {
            value += list[i].GetValue();
        }

        return value;
    }

    public WeightedOption GetWeightedRandomItem() {
        Random.InitState((int) DateTime.Now.Ticks); //I used the random to random the random...
        float rng = Random.Range(0, ListValue()+1);

        float val = 0;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].GetValue() + val >= rng) {
                return list[i];
            }
            val += list[i].GetValue();
        }

        return list[0];
    }

    public int GetListSize() {
        return list.Count;
    }
}