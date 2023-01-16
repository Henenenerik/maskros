using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Based in part on https://www.dotnetlovers.com/article/231/priority-queue
/// 
/// Priority Queue using binary heap and dictionary for quick lookup on elements and their priority.
/// </summary>
/// <typeparam name="T"></typeparam>

public class PriorityQueue<T>
{
    private float[] prios;
    private T[] elems;
    private Dictionary<T, float> elemDict;
    private int heapSize;
    private int maxSize;
    private bool isMinPrioQueue;
    public int Count { get { return heapSize; } }

    //private int cachedIndex;

    public PriorityQueue(bool isMinPrioQueue = true, int maxSize = 1000)
    {
        this.isMinPrioQueue = isMinPrioQueue;
        this.maxSize = maxSize;
        heapSize = 0;
        prios = new float[maxSize];
        elems = new T[maxSize];

        //cachedIndex = 0;
        elemDict = new Dictionary<T, float>();
    }

    public bool Enqueue(float priority, T element)
    {
        if (heapSize >= maxSize) { return false; }
        prios[heapSize] = priority;
        elems[heapSize] = element;
        heapSize += 1;
        elemDict.Add(element, priority);

        if (isMinPrioQueue)
        {
            BuildHeapMin(heapSize - 1);
        }
        else
        {
            BuildHeapMax(heapSize - 1);
        }
        return true;
    }

    public T Peek()
    {
        return elems[0];
    }

    public T Dequeue() 
    { 
        if (heapSize > 0)
        {
            T res = elems[0];
            heapSize -= 1;
            elemDict.Remove(res);
            elems[0] = elems[heapSize];
            prios[0] = prios[heapSize];
            if (isMinPrioQueue)
            {
                MinHeapify(0);
            }
            else
            {
                MaxHeapify(0);
            }
            return res;
        }
        else
        {
            throw new Exception("Dequeueing empty Priority Queue.");
        }
    }

    public (float, T) DequeueWithPrio()
    {
        if (heapSize > 0)
        {
            float prio = prios[0];
            return (prio, Dequeue());
        }
        else
        {
            throw new Exception("Dequeueing empty Priority Queue.");
        }
    }
    public void UpdatePriority(float newPriority, T element) 
    {
        /*
        if (cachedIndex < heapSize && element.Equals(elems[cachedIndex]))
        {
            prios[cachedIndex] = newPriority;
            if (isMinPrioQueue)
            {
                BuildHeapMin(cachedIndex);
                MinHeapify(cachedIndex);
            }
            else
            {
                BuildHeapMax(cachedIndex);
                MaxHeapify(cachedIndex);
            }
            return;
        }
        */
        if (!elemDict.ContainsKey(element)) { return; }
        for (int i = 0; i < heapSize; i++)
        {
            if (element.Equals(elems[i]))
            {
                prios[i] = newPriority;
                elemDict[element] = newPriority;
                if (isMinPrioQueue)
                {
                    BuildHeapMin(i);
                    MinHeapify(i);
                }
                else
                {
                    BuildHeapMax(i);
                    MaxHeapify(i);
                }
                break;
            }
        }
    }
    public bool IsInQueue(T element)
    {
        /*
        for (int i = 0; i < heapSize; i++)
        {
            if (element.Equals(elems[i]))
            {
                cachedIndex = i;
                return true;
            }
        }
        */
        return elemDict.ContainsKey(element);
    }

    public float GetPriorityForElement(T element)
    {
        /*
        if (cachedIndex < heapSize && element.Equals(elems[cachedIndex]))
        {
            return prios[cachedIndex];
        }
        */
        if (!elemDict.ContainsKey(element)) { throw new Exception("Element cannot be found"); }

        return elemDict[element];
    }

    private void MaxHeapify(int i) 
    {
        int left = i * 2 + 1;
        int right = i * 2 + 2;
        int highest = i;
        
        if (left < heapSize && prios[highest] < prios[left])
        {
            highest = left;
        }
        if (right < heapSize && prios[highest] < prios[right])
        {
            highest = right;
        }

        if (highest != i)
        {
            Swap(highest, i);
            MaxHeapify(highest);
        }
    }
    private void MinHeapify(int i)
    {
        int left = i * 2 + 1;
        int right = i * 2 + 2;
        int lowest = i;

        if (left < heapSize && prios[lowest] > prios[left])
        {
            lowest = left;
        }

        if (right < heapSize && prios[lowest] > prios[right])
        {
            lowest = right;
        }

        if (lowest != i)
        {
            Swap(lowest, i);
            MinHeapify(lowest);
        }
    }

    private void BuildHeapMax(int i)
    {
        while (i >= 0 && prios[(i - 1) / 2] < prios[i])
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }

    private void BuildHeapMin(int i)
    {
        while (i >= 0 && prios[(i - 1) / 2] > prios[i])
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }

    private void Swap(int i, int j)
    {
        T tmp = elems[i];
        elems[i] = elems[j];
        elems[j] = tmp;
        float tmp2 = prios[i];
        prios[i] = prios[j];
        prios[j] = tmp2;
    }
    
    private int Left(int i)
    {
        return i * 2 + 1;
    }

    private int Right(int i)
    {
        return i * 2 + 2;
    }
}
