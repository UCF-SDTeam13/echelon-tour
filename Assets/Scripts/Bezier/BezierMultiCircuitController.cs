using UnityEngine;

public class BezierMultiCircuitController : MonoBehaviour
{
    public BezierCircuit[] circuits;

    public BezierCircuit SetTrack(int index)
    {
        return circuits[index];
    }

    public BezierCircuit ChangeTrack(int index)
    {
        if(index == 0)
        {
            return circuits[Random.Range(index, index + 1)];
        }
        else if(index == circuits.Length - 1)
        {
            return circuits[Random.Range(index - 2, index - 1)];
        }
        else
        {
            return circuits[Random.Range(index - 1, index + 1)];
        }
    }
}
