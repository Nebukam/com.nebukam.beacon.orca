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

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Common;

namespace Nebukam.Beacon.ORCA
{
    public abstract class BaseObstacleConverter : MonoBehaviour
    {
        [Header("Settings")]
#if UNITY_EDITOR
        public bool drawDebug = true;
#endif
        [Tooltip("When true, obstacles will be registered to the default bundle.\n" +
            "Otherwise, you are responsible for registering the obstacles manually.")]
        public bool selfManaged = true;

        protected List<Nebukam.ORCA.Obstacle> m_obstacles = new List<Nebukam.ORCA.Obstacle>();
        public List<Nebukam.ORCA.Obstacle> obstacles { get { return m_obstacles; } }

    }

    public abstract class ObstacleConverter<T> : BaseObstacleConverter
    where T : Component
    {

        [Header("Collider")]
        [Tooltip("Collider Component reference. If left empty, will attempt to grab one using GetComponent()")]
        public T colliderComponent = null;
        [Header("ORCA Properties")]
        [Tooltip("Dynamic obstacles are recomputed each frame in the simulation. Check this if you intent to move or manipulate the collider often")]
        public bool dynamic = false;
        [Tooltip("If checked, obstacle vertices are recomputed each frame. Only if dynamic.")]
        public bool alwaysUpdate = false;
        [Tooltip("Whether this ORCA Obstacle collision is enabled")]
        public bool collisionEnabled = true;
        [Tooltip("Layers occupied by this Obstacle")]
        public Nebukam.ORCA.ORCALayer layerOccupation = Nebukam.ORCA.ORCALayer.ANY;
        [Tooltip("Obstacle's height [0,100]"), Range(0f, 100f)]
        public float height = 1.0f;
        [Tooltip("Obstacle's thickness [0,10]"), Range(0f, 10f)]
        public float thickness = 0.0f;

        private void Awake()
        {

            if (colliderComponent == null)
                colliderComponent = GetComponent<T>();

            if (colliderComponent == null) { return; }

            BuildObstacles();

        }

        private void OnEnable()
        {

            float baseline = transform.position.z;
            Nebukam.ORCA.Obstacle o;
            for (int i = 0, count = m_obstacles.Count; i < count; i++)
            {
                o = m_obstacles[i];

                if (selfManaged)
                {
                    if (dynamic)
                        ORCABeacon.Get.defaultBundle.dynamicObstacles.Add(o);
                    else
                        ORCABeacon.Get.defaultBundle.staticObstacles.Add(o);
                }

                o.collisionEnabled = collisionEnabled;
                o.layerOccupation = layerOccupation;
                o.baseline = baseline;
                o.height = height;
                o.thickness = thickness;
            }

        }

        private void OnDisable()
        {
            Nebukam.ORCA.Obstacle o;
            for (int i = 0, count = m_obstacles.Count; i < count; i++)
            {
                o = m_obstacles[i];
                o.collisionEnabled = false;

                if (selfManaged)
                {
                    if (dynamic)
                        ORCABeacon.Get.defaultBundle.dynamicObstacles.Remove(o);
                    else
                        ORCABeacon.Get.defaultBundle.staticObstacles.Remove(o);
                }
            }
        }

        protected int SetObstacleCount(int count)
        {
            Nebukam.ORCA.Obstacle o;
            int oCount = m_obstacles.Count;
            if (oCount == count) { return count; }
            while (oCount != count)
            {
                if (oCount < count)
                {
                    o = Pool.Rent<Nebukam.ORCA.Obstacle>();
                    m_obstacles.Add(o);
                }
                else
                {
                    o = m_obstacles.Pop();
                    o.Clear(true);
                    o.Release();
                }
                oCount = m_obstacles.Count;
            }
            return count;
        }

        protected Nebukam.ORCA.Obstacle SetObstacleLength(int index, int length)
        {
            Nebukam.ORCA.Obstacle o = m_obstacles[index];
            int oLength = o.Count;
            if (oLength == length) { return o; }
            while (oLength != length)
            {
                if (oLength < length)
                    o.Add(float3(0f));
                else
                    o.Pop(true);
                oLength = o.Count;
            }
            return o;
        }

        protected abstract void BuildObstacles();

        private void Update()
        {
            if (dynamic && alwaysUpdate && collisionEnabled) { BuildObstacles(); }
        }

        private void OnDestroy()
        {
            Nebukam.ORCA.Obstacle o;

            for (int i = 0, count = m_obstacles.Count; i < count; i++)
            {
                o = m_obstacles[i];
                o.Clear(true);
                o.Release();
            }

            m_obstacles.Clear();
        }

#if UNITY_EDITOR

        #region debug draw

        private void OnDrawGizmos()
        {
            if (!drawDebug) { return; }
            DrawDebug(Color.red);
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawDebug) { return; }
            DrawDebug(Color.green);
        }

        protected void DrawSegment(float3 A, float3 B, Color col)
        {
            float3 nrm = Maths.NormalDir(A, B, float3(0f, 0f, 1f)) * (distance(A, B) * 0.2f),
                mid = lerp(A, B, 0.5f);
            Nebukam.Common.Editor.Draw.Line(mid, mid + nrm, col.A(0.5f));
            Nebukam.Common.Editor.Draw.Line(A, B, col);
        }

        protected virtual void DrawDebug(Color col)
        {
            if (Application.isPlaying)
            {
                DrawObstacle(col);
            }
            else
            {
                T component = colliderComponent;
                if (component == null) { component = GetComponent<T>(); }
                if (component == null) { return; }
                DrawObstaclePreview(component, col);
            }
        }

        protected virtual void DrawObstacle(Color col)
        {
            for (int o = 0, oCount = m_obstacles.Count; o < oCount; o++)
            {
                Nebukam.ORCA.Obstacle subObstacle = m_obstacles[o];
                int count = subObstacle.Count;

                for (int i = 1; i < count; i++)
                    DrawSegment(subObstacle[i - 1].pos, subObstacle[i].pos, col);

                if (!subObstacle.edge)
                    DrawSegment(subObstacle[count - 1].pos, subObstacle[0].pos, col);
            }
        }

        protected abstract void DrawObstaclePreview(T component, Color col);

        #endregion

#endif

    }
}
