using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Heap<T> where T : IComparable {
    private const int DefaultCapacity = 12;
    private int currentSize;
    private T[] array;
    private HashSet<T> duplicateCheckSet;

    public Heap(int capacity = DefaultCapacity) {
        currentSize = 0;
        array = new T[capacity + 1];
        duplicateCheckSet = new HashSet<T>();
    }

    public Heap(T[] items) { // TODO remove duplicates from array
        currentSize = items.Length;
        array = new T[(currentSize + 2) * 11 / 10];
        duplicateCheckSet = new HashSet<T>(items);
        int i = 1; // use 0 index and preincrement operator instead?
        foreach (T item in items)
            array[i++] = item;
        BuildHeap();
    }

    /// <summary>
    /// Insert into the priority queue, maintaining heap order
    /// Duplicates are not allowed?
    /// </summary>
    /// <returns>false if item already in heap</returns>
    /// <param name="x">The item to insert</param>
    public bool Insert(T x) {
        if (!duplicateCheckSet.Add(x))  // checks for duplicates
            return false;               // item was not inserted, duplicate values!

        if (currentSize == array.Length - 1)
            EnlargeArray(array.Length * 2 + 1);
        
        // percolate up
        int hole = ++currentSize;
        for (array[0] = x; x.CompareTo(array[hole/2]) < 0; hole /= 2) {
            array[hole] = array[hole / 2];
        }   
        array[hole] = x;

        return true;
    }

    public T Peek() {
        if(Empty())
            throw new UnderflowException( "Heap is empty" );
        return array[1];
    }

    public T DeleteMin() {
        if (Empty())
            throw new UnderflowException("Cannot perform Delete operation on an empty Heap");

        T minItem = Peek();
        duplicateCheckSet.Remove(minItem);
        array[1] = array[currentSize--];
        PercolateDown(1);

        return minItem;
    }

    public bool Empty() {
        return currentSize == 0;
    }

    public void MakeEmpty() {
        duplicateCheckSet.Clear();;
        currentSize = 0;
    }

    public bool Contains(T item) {
        return duplicateCheckSet.Contains(item);
    }

    private void PercolateDown(int hole) {
        int child;
        T tmp = array[hole];

        for ( ; hole * 2 <= currentSize; hole = child) {
            child = hole * 2;
            if (child != currentSize && array[child + 1].CompareTo(array[child]) < 0)
                child++;
            if (array[child].CompareTo(tmp) < 0)
                array[hole] = array[child];
            else
                break;
        }

        array[hole] = tmp;
    }

    private void BuildHeap() {
        for(int i = currentSize / 2; i > 0; i--)
            PercolateDown(i);
    }

    private void EnlargeArray(int newSize) {
        T [] old = array;
        array = new T[ newSize ];
        for( int i = 0; i < old.Length; i++ )
            array[ i ] = old[ i ];
    }
}

public class UnderflowException : Exception {
    public UnderflowException(string message): base(message) { }
}