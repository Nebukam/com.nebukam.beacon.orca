using Nebukam.Utils;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Nebukam.Beacon.ORCA
{

    [AddComponentMenu("Nebukam/Beacon/ORCA/Obstacle Circle 2D")]
    public class ObstacleCircle2D : ObstacleConverter<CircleCollider2D>
    {

        [Header("Circle Sampling")]
        [Tooltip("Number of segments the circle will be converted into"), Range(3, 256)]
        public int samples = 32;

        protected override void BuildObstacles()
        {

            //TODO : Re-use existing obstacles & vertices
            Nebukam.ORCA.Obstacle obstacle = new Nebukam.ORCA.Obstacle();

            Vector2 offset = colliderComponent.offset;
            float3 pos = transform.position, refloat3 = float3(0f, colliderComponent.radius, 0f);
            quaternion rot = transform.rotation;
            float inc = Maths.TAU / samples;

            float3 Project(int sample)
            {
                float3 proj = Maths.RotateAroundPivot(float3(refloat3.x, refloat3.y, pos.z), float3(false), float3(0f, 0f, inc * sample));
                proj.x += offset.x; proj.y += offset.y; proj.z = pos.z;
                proj = Maths.RotateAroundPivot(proj, float3(false), rot);
                proj.x += pos.x; proj.y += pos.y; proj.z = pos.z;
                return proj;
            }

            for (int i = 0; i < samples; i++)
                obstacle.Add(Project(i));

            m_obstacles.Add(obstacle);

        }

#if UNITY_EDITOR

        #region debug draw

        protected override void DrawObstaclePreview(CircleCollider2D component, Color col)
        {
            
            float3 pos = transform.position, refloat3 = float3(0f, component.radius, 0f);
            quaternion rot = transform.rotation;
            float inc = Maths.TAU / samples;

            float3 Project(int sample)
            {
                float3 proj = Maths.RotateAroundPivot(float3(refloat3.x, refloat3.y, pos.z), float3(false), float3(0f, 0f, inc * sample));
                proj.x += component.offset.x; proj.y += component.offset.y; proj.z = pos.z;
                proj = Maths.RotateAroundPivot(proj, float3(false), rot);
                proj.x += pos.x; proj.y += pos.y; proj.z = pos.z;
                return proj;
            }

            float3 previous = Project(samples - 1), current;
            for (int i = 0; i < samples; i++)
            {
                current = Project(i);
                DrawSegment(previous, current, col);
                previous = current;
            }

        }

        #endregion

#endif

    }
}