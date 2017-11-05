using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour 
{
	public Map[] m_Maps;
	public int m_MapIndex;

	public Transform m_TilePrefab;
	public Transform m_ObstaclePrefab;
	public Transform m_NavMeshMaskPrefab;
	public Transform m_NavMeshFloor;
	public float m_TileSize;

	public Vector2 m_MaxMapSize;

	[Range(0, 1)]
	public float m_OutlinePercent;

	private List<Coord> m_AllTilesCoord;
	private Queue<Coord> m_ShuffledTilesCoords;

	private Map m_currentMap;

	public void GenerateMap()
	{
		m_currentMap = m_Maps [m_MapIndex];
		System.Random pRNG = new System.Random (m_currentMap.m_Seed);

		GetComponent<BoxCollider> ().size = new Vector3 (m_currentMap.m_MapSize.x * m_TileSize, 0.05f, m_currentMap.m_MapSize.y * m_TileSize);

		//Generating Coords
		m_AllTilesCoord = new List<Coord> ();

		for (int x = 0; x < m_currentMap.m_MapSize.x; x++) 
		{
			for (int y = 0; y < m_currentMap.m_MapSize.y; y++)
			{
				m_AllTilesCoord.Add (new Coord(x,y));
			}
		}

		m_ShuffledTilesCoords = new Queue<Coord> (Utility.ShuffleArray(m_AllTilesCoord.ToArray(), m_currentMap.m_Seed));


		//Create map holder object
		string holderName = "Generated Map";

		if (transform.Find(holderName)) 
		{
			DestroyImmediate (transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		//Spawning tiles
		for (int x = 0; x < m_currentMap.m_MapSize.x; x++)
		{
			for (int y = 0; y < m_currentMap.m_MapSize.y; y++) 
			{
				Vector3 tilePosition = CoordToPosition (x,y);

				Transform newTile = Instantiate (m_TilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90f)) as Transform;

				newTile.localScale = Vector3.one * (1 - m_OutlinePercent) * m_TileSize;
				newTile.parent = mapHolder;
			}
		}

		//Spawning obstacles
		bool [,] obstacleMap = new bool[(int)m_currentMap.m_MapSize.x, (int)m_currentMap.m_MapSize.y]; 

		int obstacleCount = (int)(m_currentMap.m_MapSize.x * m_currentMap.m_MapSize.y * m_currentMap.m_ObstaclePercent);
		int currentObstacleCount = 0;

		for (int i = 0; i < obstacleCount; i++)
		{
			Coord randomCoord = GetRandomCoord ();

			obstacleMap [randomCoord.x, randomCoord.y] = true;
			currentObstacleCount++;

			if (randomCoord != m_currentMap.m_MapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
			{
				float obstacleHeight = Mathf.Lerp (m_currentMap.m_MinObstacleHeight, m_currentMap.m_MaxObstacleHeight, (float)pRNG.NextDouble());
				Vector3 obstaclePosition = CoordToPosition (randomCoord.x, randomCoord.y);
				Transform newObstacle = Instantiate (m_ObstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;

				newObstacle.parent = mapHolder;
				newObstacle.localScale = new Vector3((1 - m_OutlinePercent) * m_TileSize, obstacleHeight, (1 - m_OutlinePercent) * m_TileSize);

				Renderer obstacleRenderer = newObstacle.GetComponent<Renderer> ();
				Material obstacleMaterial = new Material (obstacleRenderer.sharedMaterial);

				float colorPercent = randomCoord.y / (float)m_currentMap.m_MapSize.y;
				obstacleMaterial.color = Color.Lerp (m_currentMap.m_ForegroundColor, m_currentMap.m_BackgroundColor, colorPercent);

				obstacleRenderer.sharedMaterial = obstacleMaterial;

			} 
			else 
			{
				obstacleMap [randomCoord.x, randomCoord.y] = false;
				currentObstacleCount--;
			}
		}

		//Creating navmesh mask
		Transform leftMask = Instantiate (m_NavMeshMaskPrefab, Vector3.left * (m_currentMap.m_MapSize.x + m_MaxMapSize.x) / 4f * m_TileSize, Quaternion.identity) as Transform;
		leftMask.parent = mapHolder;
		leftMask.localScale = new Vector3 ((m_MaxMapSize.x - m_currentMap.m_MapSize.x) / 2f, 1, m_currentMap.m_MapSize.y) * m_TileSize;


		Transform rightMask = Instantiate (m_NavMeshMaskPrefab, Vector3.right * (m_currentMap.m_MapSize.x + m_MaxMapSize.x) / 4f * m_TileSize, Quaternion.identity) as Transform;
		rightMask.parent = mapHolder;
		rightMask.localScale = new Vector3 ((m_MaxMapSize.x - m_currentMap.m_MapSize.x) / 2f, 1, m_currentMap.m_MapSize.y) * m_TileSize;


		Transform topMask = Instantiate (m_NavMeshMaskPrefab, Vector3.forward * (m_currentMap.m_MapSize.y + m_MaxMapSize.y) / 4f * m_TileSize, Quaternion.identity) as Transform;
		topMask.parent = mapHolder;
		topMask.localScale = new Vector3 (m_MaxMapSize.x, 1, (m_MaxMapSize.y - m_currentMap.m_MapSize.y) / 2f) * m_TileSize;


		Transform bottomMask = Instantiate (m_NavMeshMaskPrefab, Vector3.back * (m_currentMap.m_MapSize.y + m_MaxMapSize.y) / 4f * m_TileSize, Quaternion.identity) as Transform;
		bottomMask.parent = mapHolder;
		bottomMask.localScale = new Vector3 (m_MaxMapSize.x, 1, (m_MaxMapSize.y - m_currentMap.m_MapSize.y) / 2f) * m_TileSize;


		m_NavMeshFloor.localScale = new Vector3 (m_MaxMapSize.x, m_MaxMapSize.y) * m_TileSize; 
	}

	private bool MapIsFullyAccessible(bool [,] obstacleMap, int currentObstacleCount)
	{
		bool [,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (m_currentMap.m_MapCenter);
		mapFlags [m_currentMap.m_MapCenter.x, m_currentMap.m_MapCenter.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0)
		{
			Coord tile = queue.Dequeue ();

			for (int x = -1; x <= 1; x++) 
			{
				for (int y = -1; y <= 1; y++) 
				{
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;

					if (x == 0 ^ y == 0) 
					{
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) 
						{
							if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
							{
								mapFlags [neighbourX, neighbourY] = true;
								queue.Enqueue (new Coord(neighbourX, neighbourY));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int)(m_currentMap.m_MapSize.x * m_currentMap.m_MapSize.y - currentObstacleCount);

		return targetAccessibleTileCount == accessibleTileCount;
	}


	private Vector3 CoordToPosition(int x, int y)
	{
		return new Vector3 (-m_currentMap.m_MapSize.x/2f + 0.5f + x, 0f, -m_currentMap.m_MapSize.y/2f + 0.5f + y) * m_TileSize;
	}

	public Coord GetRandomCoord()
	{
		Coord randomCoord = m_ShuffledTilesCoords.Dequeue ();
		m_ShuffledTilesCoords.Enqueue (randomCoord);
		return randomCoord;
	}

	[System.Serializable]
	public struct Coord
	{
		public int x;
		public int y;

		public Coord (int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public static bool operator == (Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator != (Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}
	}

	[System.Serializable]
	public class Map
	{
		public Coord m_MapSize;

		[Range(0, 1)]
		public float m_ObstaclePercent;

		public int m_Seed;
		public float m_MinObstacleHeight;
		public float m_MaxObstacleHeight;
		public Color m_ForegroundColor;
		public Color m_BackgroundColor;

		public Coord m_MapCenter
		{
			get	
			{
				return new Coord (m_MapSize.x / 2, m_MapSize.y / 2);
			}
		}
	}

}


