using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public enum BuildType
    {
        area,
        line
    }

public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject highlightedTile;
    [SerializeField] private BuildType buildType;
    private Camera mainCamera;
    private Vector3 lastFrameMousePosition, actualFrameMousePosition, difference;
    private Vector2Int minLevelBorder, maxLevelBorder;

    private void Start()
    {
        mainCamera = Camera.main;
        minLevelBorder = Vector2Int.zero;
        LevelController._instance.activeGridChangedCB = OnActiveGridChange;
        SimplePool.Preload(highlightedTile, 1600);
    }

    private float orthographicSize;

    private void Update()
    {
        actualFrameMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Select();
        Movement();
        lastFrameMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnActiveGridChange(Grid grid)
    {
        if (grid != null)
            maxLevelBorder = new Vector2Int(grid.width - 1, grid.height - 1);
    }

    private void Movement()
    {
        if (Input.GetMouseButton(1))
        {
            difference = lastFrameMousePosition - actualFrameMousePosition;
            mainCamera.transform.Translate(difference);
        }

        orthographicSize -= mainCamera.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        orthographicSize = Mathf.Clamp(orthographicSize, 2f, 10f);
        mainCamera.orthographicSize = orthographicSize;
    }

    private Vector2Int startPoint, endPoint, oldEndPoint;
    private int x1, y1, x2, y2, x3, y3;
    private List<GameObject> _objects = new();
    private bool[,] selectedMap;

    private void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Vector2Int.FloorToInt(actualFrameMousePosition);
            startPoint.Clamp(minLevelBorder, maxLevelBorder);
        }
        if (Input.GetMouseButtonUp(0))
        {
            startPoint = Vector2Int.zero;
            endPoint = Vector2Int.zero;
            Debug.Log($"Selected {(x2-x1+1)*(y2-y1+1)} tiles");
            while (_objects.Count > 0)
            {
                SimplePool.Despawn(_objects.First());
                _objects.RemoveAt(0);
            }
        }
        endPoint = Vector2Int.FloorToInt(actualFrameMousePosition);
        endPoint.Clamp(minLevelBorder, maxLevelBorder);
        x1 = Mathf.Min(startPoint.x, endPoint.x);
        y1 = Mathf.Min(startPoint.y, endPoint.y);
        x2 = Mathf.Max(startPoint.x, endPoint.x);
        y2 = Mathf.Max(startPoint.y, endPoint.y);
        x3 = oldEndPoint.x; y3 = oldEndPoint.y;
        if (oldEndPoint != endPoint)
        {
            while (_objects.Count > 0)
            {
                SimplePool.Despawn(_objects.First());
                _objects.RemoveAt(0);
            }
            if (Input.GetMouseButton(0))
            {
                switch (buildType)
                {
                    case BuildType.area:
                    {
                        for (var x = x1; x <= x2; x++)
                        {
                            for (var y = y1; y <= y2; y++)
                            {
                                _objects.Add(SimplePool.Spawn(highlightedTile, new Vector3(x, y, 0),
                                    Quaternion.identity));
                            }
                        }

                        break;
                    }
                    case BuildType.line:
                    {
                        if (x2 - x1 >= y2 - y1)
                        {
                            for (var x = x1; x <= x2; x++)
                            {
                                _objects.Add(SimplePool.Spawn(highlightedTile, new Vector3(x, startPoint.y, 0),
                                    Quaternion.identity));
                            }
                        }
                        else
                        {
                            for (var y = y1; y <= y2; y++)
                            {
                                _objects.Add(SimplePool.Spawn(highlightedTile, new Vector3(startPoint.x, y, 0),
                                    Quaternion.identity));
                            }
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        oldEndPoint = endPoint;
    }
}