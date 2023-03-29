using System;
using System.Collections.Generic;

// @Mark Allen Weiss - Original Java implementation
// @Author Patrik Bergsten - Translated to C#, Comparer functionality and disallow duplicates
public class Heap<T> {
    private const int DefaultCapacity = 12;
    private HashSet<T> _duplicateCheckSet;
    private IComparer<T> _comparer;
    private T[] _array;
    private int currentSize;

    public Heap() : this(null, DefaultCapacity){ }

    public Heap(int capacity) : this(null, capacity) { }

    public Heap(T[] items) {
        currentSize = items.Length;
        _array = new T[(currentSize + 2) * 11 / 10];
        _duplicateCheckSet = new HashSet<T>(items);
        int i = 1;
        foreach (T item in items)
            _array[i++] = item;
        BuildHeap();
    }
    
    /// <summary>
    /// Supply comparator if default CompareTo() not desirable.
    /// </summary>
    /// <param name="comparer">Optional parameter. Tells the heap how to evaluate <T>.</param>
    /// <param name="capacity">The desired initial size of heap array. Capacity enlarges as heap gets full.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Heap(IComparer<T> comparer = null, int capacity = DefaultCapacity) {
        currentSize = 0;
        _array = new T[capacity + 1];
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

        if (currentSize == _array.Length - 1)
            EnlargeArray(_array.Length * 2 + 1);
        
        // percolate up
        int hole = ++currentSize;
        for (_array[0] = x; _comparer.Compare(x, _array[hole / 2]) < 0; hole /= 2) { 
            //for (_array[0] = x; x.CompareTo(_array[hole/2]) < 0; hole /= 2) {
            _array[hole] = _array[hole / 2];
        }   
        _array[hole] = x;

        return true;
    }

    public T Peek() {
        if(Empty())
            throw new UnderflowException( "Heap is empty" );
        return _array[1];
    }

    public T DeleteMin() {
        if (Empty())
            throw new UnderflowException("Cannot perform Delete operation on an empty Heap");

        T minItem = Peek();
        _duplicateCheckSet.Remove(minItem);
        _array[1] = _array[currentSize--];
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
        T tmp = _array[hole];

        for ( ; hole * 2 <= currentSize; hole = child) {
            child = hole * 2;
            if (child != currentSize && _comparer.Compare(_array[child + 1], _array[child]) < 0)
                //if (child != currentSize && _array[child + 1].CompareTo(_array[child]) < 0)
                child++;
            if (_comparer.Compare(_array[child], tmp) < 0)
                //if (_array[child].CompareTo(tmp) < 0)
                _array[hole] = _array[child];
            else
                break;
        }

        _array[hole] = tmp;
    }

    private void BuildHeap() {
        for(int i = currentSize / 2; i > 0; i--)
            PercolateDown(i);
    }

    private void EnlargeArray(int newSize) {
        T [] old = _array;
        _array = new T[ newSize ];
        for( int i = 0; i < old.Length; i++ )
            _array[ i ] = old[ i ];
    }
}

public class UnderflowException : Exception {
    public UnderflowException(string message): base(message) { }
}
