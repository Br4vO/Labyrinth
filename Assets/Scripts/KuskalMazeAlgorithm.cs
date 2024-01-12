using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KuskalMazeAlgorithm : MonoBehaviour
{
    // Public 
    //------------------------------
    public Transform m_Camera;
    // Tile prefab - 1x1 square with 4 edges
    public GameObject m_Tile;
    public uint m_MazeWidth, m_MazeHeight;
    // User input wait time
    public uint m_WaitSeconds;

    // Private
    //------------------------------
    private const uint m_tileSize = 1;
    // List of edges to process
    private List<TileEdge> m_mazeEdges;
    private MazeTile[,] m_grid;
    private uint m_nrOfMazeTiles;

    // Start is called before the first frame update
    void Start()
    {
        if (InitializeAndErrorCheck())
        {
            SetupMaze();
            StartCoroutine(mazeBuilder());
        }
    }

    bool InitializeAndErrorCheck()
    {
        // Center Camera
        if (m_Camera)
        {
            m_Camera.position += new Vector3(m_MazeWidth / 2, m_MazeHeight / 2, 0);
        }
        else
        {
            Debug.Log("No Camera centering");
        }

        // Verify User input
        if (m_MazeWidth == 0 || m_MazeHeight == 0)
        {
            Debug.LogError("Width or Height not valid");
            return false;
        }
        
        if (!m_Tile)
        {
            Debug.LogError("Tile GameObject is NULL");
            return false;
        }

        m_nrOfMazeTiles = m_MazeWidth * m_MazeHeight;

        // Initialize
        m_mazeEdges = new List<TileEdge>((int)m_nrOfMazeTiles * 2);
        m_grid = new MazeTile[m_MazeWidth, m_MazeHeight];

        return ((m_mazeEdges != null) && (m_grid != null));
    }

    void SetupMaze()
    {
        List<int> availableTileIndicies = new List<int>();

        // We are randomizing the edges by randomly grabbing indices from availableTileIndicies
        for (int i = 0; i < (m_nrOfMazeTiles * 2); i++)
        {
            availableTileIndicies.Add(i);
            m_mazeEdges.Add(new TileEdge());
        }

        int randomIndex;
        for (uint width = 0; width < m_MazeWidth; width++)
        {
            for (uint height = 0; height < m_MazeHeight; height++)
            {
                // Initialize Tiles for Maze
                // Tile transform.position is m_grid indexing
                GameObject tileObject = Instantiate(m_Tile, new Vector3(width, height, 0), Quaternion.identity);
                m_grid[width, height] = new MazeTile();
                m_grid[width, height].SetTileObject(tileObject);

                // We are only grabbing North and West edges. South and East will be the oppsite edges when building the maze
                // Randomly grabbing North edge
                randomIndex = Random.Range(0, availableTileIndicies.Count);
                m_mazeEdges[availableTileIndicies[randomIndex]] = m_grid[width, height].getEdge(EdgeDirection.North);
                availableTileIndicies.RemoveAt(randomIndex);
                // Randomly grabbing West edge
                randomIndex = Random.Range(0, availableTileIndicies.Count);
                m_mazeEdges[availableTileIndicies[randomIndex]] = m_grid[width, height].getEdge(EdgeDirection.West);
                availableTileIndicies.RemoveAt(randomIndex);
            }
        }
    }

    // Mazebuilder coroutine runs until we have checked all edges
    IEnumerator mazeBuilder()
    {
        while(m_mazeEdges.Count > 0) 
        {
            // Grab edge and verify
            TileEdge edge = m_mazeEdges[0];
            m_mazeEdges.RemoveAt(0);
            if (edge == null ) 
            {
                Debug.LogError("Null edge discovered.");
                yield break;
            }
            
            // Get parent mazeTile
            MazeTile mazeTile = m_grid[(int)edge.m_ParentTilePosition.x, (int)edge.m_ParentTilePosition.y];
            Vector2 neighborTileIndex = edge.GetNeighboorTileIndex();

            // Check if tile opposite edge exists
            if (neighborTileIndex.x < m_MazeWidth && neighborTileIndex.y < m_MazeHeight && neighborTileIndex.x >= 0 && neighborTileIndex.y >= 0)
            {
                MazeTile neighborTile = m_grid[(int)neighborTileIndex.x, (int)neighborTileIndex.y];

                // Check if maze tiles are already connected in some way to avoid loops
                if (!(mazeTile.m_TreeNode.IsConnected(neighborTile.m_TreeNode)))
                {
                    // Connect the tiles to the same tree and remove edges
                    mazeTile.m_TreeNode.Connect(ref neighborTile.m_TreeNode);
                    mazeTile.RemoveEdge(edge);
                    neighborTile.RemoveEdge(neighborTile.getOppositeEdge(edge.m_Direction));

                    yield return new WaitForSeconds(m_WaitSeconds);
                }
            }
        }

        Debug.Log("Finished building labyrinth!");
    }
}
