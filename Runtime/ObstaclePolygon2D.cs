using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Signals;
using Nebukam.Common;
using Nebukam.Utils;

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
                float3 proj = Maths.RotateAroundPivot(float3(pt.x + offset.x, pt.y + offset.y, pos.z), float3(false), rot);
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
                float3 proj = Maths.RotateAroundPivot(float3(pt.x + offset.x, pt.y + offset.y, pos.z), float3(false), rot);
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