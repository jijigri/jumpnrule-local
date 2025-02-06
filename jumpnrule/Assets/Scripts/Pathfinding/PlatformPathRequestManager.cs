using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPathRequestManager : MonoBehaviour
{
    Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
    PathRequest _currentPathRequest;

    static PlatformPathRequestManager instance;
    PlatformPathfinder pathfinder;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinder = GetComponent<PlatformPathfinder>();
    }

    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<WayPoint[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance._pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if(!isProcessingPath && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinder.StartFindPath(_currentPathRequest.pathStart, _currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(WayPoint[] path, bool success)
    {
        _currentPathRequest.callback?.Invoke(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<WayPoint[], bool> callback;

        public PathRequest(Vector2 _start, Vector2 _end, Action<WayPoint[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
