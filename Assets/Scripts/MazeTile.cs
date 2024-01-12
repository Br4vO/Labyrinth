using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Enum for edge relative to parent tile
public enum EdgeDirection
{
    North = 0,
    South = 1,
    West = 2,
    East = 3,
    DirDefault = 4
}

// Simple tree node to keep track of maze
public class MazeTreeNode
{
    // Private
    //------------------------------
    private MazeTreeNode m_parent = null;

    public MazeTreeNode GetRoot()
    {
        if (m_parent != null)
            return m_parent.GetRoot();
        else
            return this;
    }

    public void Connect(ref MazeTreeNode i_child) 
    {
        i_child.GetRoot().m_parent = this; 
    }
    public bool IsConnected(MazeTreeNode i_node) 
    {
        return this.GetRoot() == i_node.GetRoot(); 
    }
}

public class TileEdge
{
    // Public 
    //------------------------------
    public Transform m_EdgeTransform = null;
    public EdgeDirection m_Direction = EdgeDirection.DirDefault;
    public Vector2 m_ParentTilePosition;

    // Private
    //------------------------------
    private readonly Vector2 m_directionToIndex;

    public TileEdge() { }

    public TileEdge(Transform i_edgeTransform, EdgeDirection i_direction)
    {
        m_EdgeTransform = i_edgeTransform;
        m_Direction = i_direction;
        m_ParentTilePosition = i_edgeTransform.parent.position;

        // For easy indexing into neighbor tile
        switch (m_Direction)
        {
            case EdgeDirection.North:
                m_directionToIndex = new Vector2(0f, 1f);
                break;
            case EdgeDirection.South:
                m_directionToIndex = new Vector2(0f, -1f);
                break;
            case EdgeDirection.West:
                m_directionToIndex = new Vector2(-1f, 0f);
                break;
            case EdgeDirection.East:
                m_directionToIndex = new Vector2(1f, 0f);
                break;
            default:
                Debug.LogError("Direction not valid.");
                break;
        }
    }

    public Vector2 GetNeighboorTileIndex()
    {
        return new Vector2(m_ParentTilePosition.x + m_directionToIndex.x, m_ParentTilePosition.y + m_directionToIndex.y);
    }
}

public class MazeTile
{
    public GameObject m_TileObject;
    public TileEdge[] m_edges;
    public MazeTreeNode m_TreeNode;

    public void SetTileObject(GameObject i_tileObject) 
    { 
        m_TileObject = i_tileObject;

        m_edges = new TileEdge[4];
        m_edges[(int)EdgeDirection.North] = new TileEdge(i_tileObject.transform.Find("Top"), EdgeDirection.North);
        m_edges[(int)EdgeDirection.South] = new TileEdge(i_tileObject.transform.Find("Bottom"), EdgeDirection.South);
        m_edges[(int)EdgeDirection.West] = new TileEdge(i_tileObject.transform.Find("Left"), EdgeDirection.West);
        m_edges[(int)EdgeDirection.East] = new TileEdge(i_tileObject.transform.Find("Right"), EdgeDirection.East);

        m_TreeNode = new MazeTreeNode();
    }

    public TileEdge getEdge(EdgeDirection i_dir) 
    {
        switch (i_dir) 
        {
            case EdgeDirection.North:
                return m_edges[(int)EdgeDirection.North];
            case EdgeDirection.South:
                return m_edges[(int)EdgeDirection.South];
            case EdgeDirection.West:
                return m_edges[(int)EdgeDirection.West];
            case EdgeDirection.East:
                return m_edges[(int)EdgeDirection.East];
            default:
                Debug.LogError("Direction not valid.");
                return null;
        }
    }

    public TileEdge getOppositeEdge(EdgeDirection i_dir) 
    { 
        switch (i_dir) 
        {
            case EdgeDirection.North:
                return m_edges[(int)EdgeDirection.South];
            case EdgeDirection.South:
                return m_edges[(int)EdgeDirection.North];
            case EdgeDirection.West:
                return m_edges[(int)EdgeDirection.East];
            case EdgeDirection.East:
                return m_edges[(int)EdgeDirection.West];
            default:
                Debug.LogError("Direction not valid.");
                return null;
        }
    }

    public bool RemoveEdge(TileEdge i_edge)
    {
        switch (i_edge.m_Direction)
        {
            case EdgeDirection.North:
                m_edges[(int)EdgeDirection.North].m_EdgeTransform.gameObject.SetActive(false);
                m_edges[(int)EdgeDirection.North] = null;
                return true;
            case EdgeDirection.South:
                m_edges[(int)EdgeDirection.South].m_EdgeTransform.gameObject.SetActive(false);
                m_edges[(int)EdgeDirection.South] = null;
                return true;
            case EdgeDirection.West:
                m_edges[(int)EdgeDirection.West].m_EdgeTransform.gameObject.SetActive(false);
                m_edges[(int)EdgeDirection.West] = null;
                return true;
            case EdgeDirection.East:
                m_edges[(int)EdgeDirection.East].m_EdgeTransform.gameObject.SetActive(false);
                m_edges[(int)EdgeDirection.East] = null;
                return true;
            default:
                Debug.LogError("Could not remove edge.");
                return false;
        }
    }
}