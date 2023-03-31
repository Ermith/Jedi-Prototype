using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Segment
{
    public Segment(Vector3 top, Vector3 bottom)
    {
        Top = top;
        Bottom = bottom;
    }
    public Vector3 Top;
    public Vector3 Bottom;
    public Vector3 Mid => (Top - Bottom) / 2 + Bottom;
    public (Vector3 top, Vector3 bottom) Shrunk(float scale)
    {
        Vector3 up = Top - Mid;
        Vector3 top = Mid + up * scale;
        Vector3 bottom = Mid - up * scale;
        return (top, bottom);
    }
}

public class LightsaberTrail : MonoBehaviour
{
    private const int NUM_VERTICES = 4;
    private const int NUM_TRIANGLES_INDICES = 12;

    [SerializeField, Tooltip("Big width means longer weapon.")]
    public float Width;

    [SerializeField, Tooltip("Mostly for performance. Every FixedUpdate movement, new segment is created and the trail extends to that segment.")]
    private int _maxSegments = 2;

    [SerializeField, Tooltip("Restricts segments to be of maximum cumulative length. So lower FPS does not cause HUGE trails.")]
    private float _maxTrailLength;


    [SerializeField]
    private Material _material;

    private GameObject _meshParent;
    private Mesh _mesh;
    private LinkedList<Segment> _segments;
    private Vector3 _bottom => transform.localPosition;
    private Vector3 _top => _bottom + transform.up * Width;
    public bool EnableTrail = false;

    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
        _meshParent = new GameObject();
        _meshParent.AddComponent<MeshFilter>().mesh = _mesh;
        _meshParent.AddComponent<MeshRenderer>().material = _material;

        _segments = new LinkedList<Segment>();
        _segments.AddFirst(new Segment(_top, _bottom));
    }

    private void FixedUpdate()
    {
        _meshParent.transform.position = transform.position;
        Segment previousSegment = _segments.First.Value;
        Segment segment = new Segment(_top, _bottom);
        _segments.AddFirst(segment);

        if (!EnableTrail)
        {
            foreach (Segment s in _segments)
            {
                s.Top = _bottom;
                s.Bottom = _top;
            }
        }

        _meshParent.GetComponent<MeshRenderer>().enabled = EnableTrail;
    }

    void LateUpdate()
    {
        try
        {
            _segments.AddFirst(new Segment(_top, _bottom));

            while (_segments.Count > _maxSegments)
                _segments.RemoveLast();

            var vertices = new Vector3[_maxSegments * 2];
            var triangles = new int[(_maxSegments - 1) * NUM_TRIANGLES_INDICES];

            ResolveTrail(vertices, triangles);

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
        } catch
        {
            Debug.Log("WTF");
        }
    }

    private void SetQuadIndices(int[] triangles, int vertexArrayOffset, int triangleArrayOffset)
    {
        // First triangle (Both sides)
        triangles[triangleArrayOffset + 0] = vertexArrayOffset + 0;
        triangles[triangleArrayOffset + 1] = vertexArrayOffset + 1;
        triangles[triangleArrayOffset + 2] = vertexArrayOffset + 2;

        triangles[triangleArrayOffset + 3] = vertexArrayOffset + 1;
        triangles[triangleArrayOffset + 4] = vertexArrayOffset + 0;
        triangles[triangleArrayOffset + 5] = vertexArrayOffset + 2;

        // Second triangle (Both sides)
        triangles[triangleArrayOffset + 6] = vertexArrayOffset + 1;
        triangles[triangleArrayOffset + 7] = vertexArrayOffset + 2;
        triangles[triangleArrayOffset + 8] = vertexArrayOffset + 3;

        triangles[triangleArrayOffset + 09] = vertexArrayOffset + 2;
        triangles[triangleArrayOffset + 10] = vertexArrayOffset + 1;
        triangles[triangleArrayOffset + 11] = vertexArrayOffset + 3;
    }

    private float TrailLength()
    {
        LinkedListNode<Segment> node = _segments.First;
        float totalDistance = 0;
        while (node.Next != null)
        {
            totalDistance += (node.Value.Mid - node.Next.Value.Mid).magnitude;
            node = node.Next;
        }

        return totalDistance;
    }

    private void ResolveTrail(Vector3[] vertices, int[] triangles)
    {
        int v = 1;
        float totalLength = TrailLength();
        float segmentLength = Mathf.Min(totalLength, _maxTrailLength) / _maxSegments;
        float untilNextSegment = segmentLength;
        Segment previousSegment = null;
        foreach (Segment segment in _segments)
        {
            if (previousSegment == null)
            {
                vertices[v - 1] = segment.Top;
                vertices[v] = segment.Bottom;
                v += 2;
            } else
            {
                float length = (segment.Mid - previousSegment.Mid).magnitude;
                untilNextSegment -= length;

                while (untilNextSegment < 0)
                {
                    float t = (length + untilNextSegment) / length;
                    Vector3 top = Vector3.Lerp(previousSegment.Top, segment.Top, t);
                    Vector3 bottom = Vector3.Lerp(previousSegment.Bottom, segment.Bottom, t);
                    untilNextSegment += segmentLength;

                    vertices[v - 1] = segment.Top;
                    vertices[v] = segment.Bottom;

                    SetQuadIndices(triangles, v - 3, NUM_TRIANGLES_INDICES * (v - 3) / 2);
                    v += 2;

                    if (v > vertices.Length)
                        return;
                }

            }

            previousSegment = segment;
        }
    }
}
