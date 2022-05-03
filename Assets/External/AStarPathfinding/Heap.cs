using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A-star pathfinding code taken from IMAT2904 AI For Simulation provided code.
//All credit Salim Hasshu

public class Heap<T> where T : IHeapElement<T>
{

    T[] array;
    int currentLength;

    public Heap(int heapSize)
    {
        array = new T[heapSize];
    }

    public void Add(T element)
    {
        element.HeapIndex = currentLength;
        array[currentLength] = element;
        SortUp(element);
        currentLength++;
    }

    void SortUp(T element)
    {
        int parentIndex = (element.HeapIndex - 1) / 2;

        while (true)
        {
            T parentElement = array[parentIndex];
            if(element.CompareTo(parentElement) > 0)
            {
                Swap(element, parentElement);
            }
            else
            {
                break;
            }

            parentIndex = (element.HeapIndex - 1) / 2;
        }
    }

    public bool Contains(T element)
    {
        return Equals(array[element.HeapIndex], element);
    }

    public int Count
    {
        get
        {
            return currentLength;
        }
    }

    public void UpdateElement(T element)
    {
        SortUp(element);
    }

    void Swap(T elementA, T elementB)
    {
        array[elementA.HeapIndex] = elementB;
        array[elementB.HeapIndex] = elementA;
        int elementAIndex = elementA.HeapIndex;
        elementA.HeapIndex = elementB.HeapIndex;
        elementB.HeapIndex = elementAIndex;
    }

    public T RemoveTop()
    {
        T topElement = array[0];
        currentLength--;
        array[0] = array[currentLength];
        array[0].HeapIndex = 0;
        SortDown(array[0]);
        return topElement;
    }

    void SortDown(T element)
    {
        while (true)
        {
            int childIndexLeft = element.HeapIndex * 2 + 1;
            int childIndexRight = element.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if(childIndexLeft < currentLength)
            {
                swapIndex = childIndexLeft;
                if(childIndexRight < currentLength)
                {
                    if (array[childIndexLeft].CompareTo(array[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if(element.CompareTo(array[swapIndex]) < 0)
                {
                    Swap(element, array[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

}


public interface IHeapElement<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}