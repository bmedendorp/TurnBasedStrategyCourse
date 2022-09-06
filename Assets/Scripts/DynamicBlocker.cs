using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DynamicBlocker : MonoBehaviour
{
    public static event EventHandler OnAnyBlock;
    public static event EventHandler OnAnyUnblock;

    public class BlockerEventArgs : EventArgs
    {
        public GraphNode graphNode;

        public BlockerEventArgs(GraphNode graphNode)
        {
            this.graphNode = graphNode;
        }
    }

	private GraphNode lastBlocked;

    protected virtual void Start()
    {
        BlockAtCurrentPosition();        
    }

    protected virtual void OnDestroy()
    {
        Unblock();
    }

    protected void BlockAtCurrentPosition () 
    {
        BlockAt(transform.position);
    }

    protected void BlockAt (Vector3 position) 
    {
        Unblock();
        var node = AstarPath.active.GetNearest(position, NNConstraint.None).node;
        if (node != null) {
            Block(node);
        }
    }

    protected void Block (GraphNode node) 
    {
        if (node == null)
            throw new System.ArgumentNullException("node");

        OnAnyBlock?.Invoke(this, new BlockerEventArgs(node));
        lastBlocked = node;
    }

    /// <summary>Unblock the last node that was blocked (if any)</summary>
    protected void Unblock () {
        if (lastBlocked == null || lastBlocked.Destroyed) {
            lastBlocked = null;
            return;
        }

        OnAnyUnblock?.Invoke(this, new BlockerEventArgs(lastBlocked));
        lastBlocked = null;
    }
}
