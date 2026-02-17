using UnityEngine;

public class DisjointSet
{
    private int[] elements;

    public DisjointSet(int size)
    {
        elements = new int[size];
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = -1;
        }
    }

    public void union(int rootA, int rootB)
    {
        if (elements[rootA] < elements[rootB])      // rootB is deeper
            elements[rootA] = rootB;                // rootB is new parent of rootA
        else
        {
            if (elements[rootA] == elements[rootB]) // update height if same
                elements[rootA]--;
            elements[rootB] = rootA;                // rootA is new parent of rootB
        }
    }

    /**
     * Finds the root of the set that x belongs to
     */
    public int find(int x)
    {
        if (elements[x] < 0) 
            return x;
        else
            return elements[x] = find(elements[x]);
    }
}
