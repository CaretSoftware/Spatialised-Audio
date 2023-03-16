using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Heap<T> {
    private const int DefaultCapacity = 12;
    private int currentSize;
    private T[] array;
    private HashSet<T> _duplicateCheckSet;
    private IComparer<T> _comparer;

    public Heap(T[] items) { // TODO remove duplicates from array
        currentSize = items.Length;
        array = new T[(currentSize + 2) * 11 / 10];
        _duplicateCheckSet = new HashSet<T>(items);
        int i = 1; // use 0 index and preincrement operator instead?
        foreach (T item in items)
            array[i++] = item;
        BuildHeap();
    }
    
    public Heap(IComparer<T> comparer = null, int capacity = DefaultCapacity) {
        currentSize = 0;
        array = new T[capacity + 1];
        _duplicateCheckSet = new HashSet<T>();
        
        if (comparer == null)
            if (typeof(IComparable).IsAssignableFrom(typeof(T)) ||
                typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                comparer = Comparer<T>.Default;

        if (comparer == null)
            throw new ArgumentNullException(
                nameof(comparer),
                $"There's no default comparer for {typeof(T).Name} class, you should provide it explicitly.");
        
        _comparer = comparer;
    }

    /// <summary>
    /// Insert into the priority queue, maintaining heap order
    /// Duplicates are not allowed.
    /// </summary>
    /// <returns>false if item already in heap</returns>
    /// <param name="x">The item to insert</param>
    public bool Insert(T x) {
        if (!_duplicateCheckSet.Add(x))  // checks for duplicates
            return false;               // item was not inserted, duplicate values!

        if (currentSize == array.Length - 1)
            EnlargeArray(array.Length * 2 + 1);
        
        // percolate up
        int hole = ++currentSize;
        for (array[0] = x; _comparer.Compare(x, array[hole / 2]) < 0; hole /= 2) { 
            //for (array[0] = x; x.CompareTo(array[hole/2]) < 0; hole /= 2) {
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
        _duplicateCheckSet.Remove(minItem);
        array[1] = array[currentSize--];
        PercolateDown(1);

        return minItem;
    }

    public bool Empty() {
        return currentSize == 0;
    }

    public void MakeEmpty() {
        _duplicateCheckSet.Clear();;
        currentSize = 0;
    }

    public bool Contains(T item) {
        return _duplicateCheckSet.Contains(item);
    }

    private void PercolateDown(int hole) {
        int child;
        T tmp = array[hole];

        for ( ; hole * 2 <= currentSize; hole = child) {
            child = hole * 2;
            if (child != currentSize && _comparer.Compare(array[child + 1], array[child]) < 0)
                //if (child != currentSize && array[child + 1].CompareTo(array[child]) < 0)
                child++;
            if (_comparer.Compare(array[child], tmp) < 0)
                //if (array[child].CompareTo(tmp) < 0)
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