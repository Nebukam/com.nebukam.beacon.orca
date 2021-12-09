// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
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

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Common;

namespace Nebukam.Beacon.ORCA
{

    [AddComponentMenu("Nebukam/Beacon/ORCA/Obstacle Polygon 2D")]
    public class ObstaclePolygon2D : ObstacleConverter<PolygonCollider2D>
    {

        protected override void BuildObstacles()
        {

            int pathCount = SetObstacleCount(colliderComponent.pathCount);

            Vector2[] points;
            Vector2 offset = colliderComponent.offset;
            float3 pos = transform.position;
            quaternion rot = transform.rotation;

            float3 Project(Vector2 pt)
            {
                float3 proj = Maths.RotateAroundPivot(float3(pt.x + offset.x, pt.y + offset.y, pos.z), float3(0f), rot);
                proj.x += pos.x; proj.y += pos.y; proj.z = pos.z;
                return proj;
            }

            Nebukam.ORCA.Obstacle o;
            int length;
            for (int p = 0; p < pathCount; p++)
            {
                points = colliderComponent.GetPath(p);
                length = points.Length;
                o = SetObstacleLength(p, length);

                for (int i = 0; i < length; i++)
                    o[i].pos = Project(points[i]);

            }

        }

#if UNITY_EDITOR

        #region debug draw

        protected override void DrawObstaclePreview(PolygonCollider2D component, Color col)
        {
            int pathCount = component.pathCount;

            Vector2[] points;
            Vector2 offset = component.offset;
            float3 pos = transform.position;
            quaternion rot = transform.rotation;

            float3 Project(Vector2 pt)
            {
                float3 proj = Maths.RotateAroundPivot(float3(pt.x + offset.x, pt.y + offset.y, pos.z), float3(0f), rot);
                proj.x += pos.x; proj.y += pos.y; proj.z = pos.z;
                return proj;
            }

            for (int p = 0; p < pathCount; p++)
            {
                points = component.GetPath(p);
                float3 previous = Project(points[points.Length - 1]), current;
                for (int i = 0, count = points.Length; i < count; i++)
                {
                    current = Project(points[i]);
                    DrawSegment(previous, current, col);
                    previous = current;
                }
            }
        }

        #endregion

#endif

    }
}