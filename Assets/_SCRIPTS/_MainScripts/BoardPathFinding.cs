using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class BoardPathfinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14; // Used only if you enable diagonal movement

        // Wrapper class to keep your original CellData clean
        private class AStarNode
        {
            public int x;
            public int y;
            public int gCost;
            public int hCost;
            public int fCost => gCost + hCost;
            public AStarNode parent;

            public AStarNode(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// Calculates the shortest path between two points on your _boardData grid.
        /// Returns a list of Vector2Int coordinates from start to end, or null if blocked.
        /// </summary>
        public List<Vector2Int> FindPath(BoardManager.CellData[,] boardData, Vector2Int startPos, Vector2Int endPos,
            bool allowDiagonal = false)
        {
            int width = boardData.GetLength(0);
            int height = boardData.GetLength(1);

            // Validation bounds check
            if (startPos.x < 0 || startPos.x >= width || startPos.y < 0 || startPos.y >= height) return null;
            if (endPos.x < 0 || endPos.x >= width || endPos.y < 0 || endPos.y >= height) return null;
            if (!boardData[endPos.x, endPos.y].Passable) return null; // Destination is blocked

            // Runtime node tracker matrix
            AStarNode[,] nodeGrid = new AStarNode[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodeGrid[x, y] = new AStarNode(x, y) { gCost = int.MaxValue };
                }
            }

            AStarNode startNode = nodeGrid[startPos.x, startPos.y];
            AStarNode endNode = nodeGrid[endPos.x, endPos.y];

            List<AStarNode> openList = new List<AStarNode> { startNode };
            HashSet<AStarNode> closedList = new HashSet<AStarNode>();

            startNode.gCost = 0;
            startNode.hCost = GetHeuristicDistance(startNode, endNode, allowDiagonal);

            while (openList.Count > 0)
            {
                AStarNode currentNode = GetLowestFCostNode(openList);

                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (AStarNode neighbor in GetNeighbors(nodeGrid, currentNode, boardData, width, height,
                             allowDiagonal))
                {
                    if (closedList.Contains(neighbor)) continue;

                    int moveCost = (currentNode.x == neighbor.x || currentNode.y == neighbor.y)
                        ? MOVE_STRAIGHT_COST
                        : MOVE_DIAGONAL_COST;
                    int tentativeGCost = currentNode.gCost + moveCost;

                    if (tentativeGCost < neighbor.gCost)
                    {
                        neighbor.parent = currentNode;
                        neighbor.gCost = tentativeGCost;
                        neighbor.hCost = GetHeuristicDistance(neighbor, endNode, allowDiagonal);

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return null; // Path completely blocked
        }

        private AStarNode GetLowestFCostNode(List<AStarNode> list)
        {
            AStarNode lowest = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].fCost < lowest.fCost || (list[i].fCost == lowest.fCost && list[i].hCost < lowest.hCost))
                {
                    lowest = list[i];
                }
            }

            return lowest;
        }

        private int GetHeuristicDistance(AStarNode a, AStarNode b, bool allowDiagonal)
        {
            int dX = Mathf.Abs(a.x - b.x);
            int dY = Mathf.Abs(a.y - b.y);

            if (allowDiagonal)
            {
                // Diagonal distance (Chebyshev/Octile)
                int remaining = Mathf.Abs(dX - dY);
                return (MOVE_DIAGONAL_COST * Mathf.Min(dX, dY)) + (MOVE_STRAIGHT_COST * remaining);
            }

            // Manhattan distance for cross-only movement
            return (dX + dY) * MOVE_STRAIGHT_COST;
        }

        private List<AStarNode> GetNeighbors(AStarNode[,] nodeGrid, AStarNode curr, BoardManager.CellData[,] board, int w, int h,
            bool allowDiagonal)
        {
            List<AStarNode> neighbors = new List<AStarNode>();

            // Loop from -1 to 1 for x and y axes
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    if (xOffset == 0 && yOffset == 0) continue; // Skip self
                    if (!allowDiagonal && xOffset != 0 && yOffset != 0) continue; // Skip diagonals if restricted

                    int checkX = curr.x + xOffset;
                    int checkY = curr.y + yOffset;

                    // Ensure it is inside the board boundaries and Passable
                    if (checkX >= 0 && checkX < w && checkY >= 0 && checkY < h)
                    {
                        if (board[checkX, checkY].Passable)
                        {
                            neighbors.Add(nodeGrid[checkX, checkY]);
                        }
                    }
                }
            }

            return neighbors;
        }

        private List<Vector2Int> RetracePath(AStarNode startNode, AStarNode endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            AStarNode current = endNode;

            while (current != startNode)
            {
                path.Add(new Vector2Int(current.x, current.y));
                current = current.parent;
            }

            path.Reverse();
            return path;
        }
    }
}