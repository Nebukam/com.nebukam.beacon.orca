using Nebukam.Utils;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Nebukam.Beacon.ORCA
{
    public abstract class BaseObstacleConverter : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool drawDebug = true;
#endif
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
        [Tooltip("Dynamic obstacles are recomputed each frame. Check this if you intent to move or manipulate the collider often")]
        public bool dynamic = false;
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
                o.collisionEnabled = collisionEnabled;
                o.layerOccupation = layerOccupation;
                o.baseline = baseline;
                o.height = height;
                o.thickness = thickness;
            }

        }

        private void OnDisable()
        {
            for (int i = 0, count = m_obstacles.Count; i < count; i++)
                m_obstacles[i].collisionEnabled = false;
        }

        protected int SetObstacleCount(int count)
        {
            int oCount = m_obstacles.Count;
            if(oCount == count) { return count; }
            while(oCount != count)
            {
                if(oCount < count)
                    m_obstacles.Add(Pooling.Pool.Rent<Nebukam.ORCA.Obstacle>());
                else
                    m_obstacles.Pop().Clear(true);
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
                    o.Add(float3(false));
                else
                    o.Pop(true);
                oLength = o.Count;
            }
            return o;
        }

        protected abstract void BuildObstacles();

        private void Update()
        {
            if (dynamic) { BuildObstacles(); }
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
            Draw.Line(mid, mid + nrm, col.A(0.5f));
            Draw.Line(A, B, col);
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

                if(!subObstacle.edge)
                    DrawSegment(subObstacle[count - 1].pos, subObstacle[0].pos, col);
            }
        }

        protected abstract void DrawObstaclePreview(T component, Color col);

        #endregion

#endif

    }
}
