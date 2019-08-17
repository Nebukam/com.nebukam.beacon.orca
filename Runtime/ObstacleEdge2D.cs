// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Nebukam.Utils;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Nebukam.Beacon.ORCA
{

    [AddComponentMenu("Nebukam/Beacon/ORCA/Obstacle Edge 2D")]
    public class ObstacleEdge2D : ObstacleConverter<EdgeCollider2D>
    {
        
        [Header("Edge (experimental)")]
        [Tooltip("[EXPERIMENTAL] Double-sided edge collides from both sides.")]
        public bool doubleSided = false;

        protected override void BuildObstacles()
        {

            SetObstacleCount(doubleSided ? 2 : 1);

            Vector2[] points = colliderComponent.points;
            Vector2 offset = colliderComponent.offset;
            float3 pos = transform.position;
            quaternion rot = transform.rotation;
            int count = points.Length;

            Nebukam.ORCA.Obstacle o = SetObstacleLength(0, count);
            o.edge = true;

            float3 Project(Vector2 pt)
            {
                float3 proj = Maths.RotateAroundPivot(float3(pt.x + offset.x, pt.y + offset.y, pos.z), float3(false), rot);
                proj.x += pos.x; proj.y += pos.y; proj.z = pos.z;
                return proj;
            }

            for (int i = 0; i < count; i++)
                o[i].pos = Project(points[i]);

            if (doubleSided)
            {
                float3 overlap = float3(float.Epsilon, float.Epsilon, float.Epsilon);
                o = SetObstacleLength(1, count);
                o.edge = true;
                for (int i = 0; i < count; i++)
                    o[i].pos = Project(points[count-(i+1)]) + overlap;
            }

        }

#if UNITY_EDITOR

        #region debug draw

        protected override void DrawObstaclePreview(EdgeCollider2D component, Color col)
        {
            EdgeCollider2D CC = colliderComponent;
            if (CC == null) { CC = GetComponent<EdgeCollider2D>(); }
            if (CC == null) { return; }

            Vector2[] points;
            Vector2 offset = CC.offset;
            float3 pos = transform.position;
            quaternion rot = transform.rotation;

            points = CC.points;

            float3 Project(Vector2 pt)
            {
                float3 proj = Maths.RotateAroundPivot(float3(pt.x + offset.x, pt.y + offset.y, pos.z), float3(false), rot);
                proj.x += pos.x; proj.y += pos.y; proj.z = pos.z;
                return proj;
            }

            for (int i = 1, count = points.Length; i < count; i++)
                DrawSegment(Project(points[i - 1]), Project(points[i]), col);

            if(doubleSided)
            {
                for (int i = 1, count = points.Length; i < count; i++)
                    DrawSegment(Project(points[count - i]), Project(points[count - (i+1)]), col);
            }

        }
    
        #endregion

#endif

    }
}