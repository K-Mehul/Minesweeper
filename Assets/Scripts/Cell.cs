using UnityEngine;

public struct Cell 
{
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revelead;
    public bool flagged;
    public bool exploded;
}
