using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Signals;
using Nebukam.Common;
using Nebukam.ORCA;

namespace Nebukam.Beacon.ORCA
{
    [AddComponentMenu("Nebukam/Beacon/ORCA/ORCA Beacon Processor")]
    public class ORCABeaconProcessor : BeaconProcessor
    {

        public AxisPair plane = AxisPair.XY;

        public float radius = 0.5f;
        public float radiusObst = 0.5f;
        public float maxSpeed = 20.0f;

        public int maxNeighbors = 15;
        public float neighborDist = 20.0f;

        public float timeHorizon = 15.0f;
        public float timeHorizonObst = 1.2f;

        public ORCALayer layerOccupation = ORCALayer.ALL;
        public ORCALayer layerIgnore = ORCALayer.NONE;
        public bool navigationEnabled = true;
        public bool collisionEnabled = true;

        protected Nebukam.ORCA.Agent m_agent = null;
        public Nebukam.ORCA.Agent agent {
            get { return m_agent; }
            set
            {
                if(m_agent == value) { return; }
                m_agent = value;
                CommitAgentSettings();
            }
        }

        public void CommitAgentSettings()
        {
            if(m_agent == null) { return; }

            m_agent.radius = radius;
            m_agent.radiusObst = radiusObst;
            m_agent.maxSpeed = maxSpeed;
            m_agent.maxNeighbors = maxNeighbors;
            m_agent.neighborDist = neighborDist;
            m_agent.timeHorizon = timeHorizon;
            m_agent.timeHorizonObst = timeHorizonObst;
            m_agent.layerOccupation = layerOccupation;
            m_agent.layerIgnore = layerIgnore;
            m_agent.navigationEnabled = navigationEnabled;
            m_agent.collisionEnabled = collisionEnabled;
        }

        private void OnEnable()
        {
            if (m_agent == null) { return; }

            float3 pos = transform.position;
            m_agent.pos = pos;
            m_agent.navigationEnabled = navigationEnabled;
            m_agent.collisionEnabled = collisionEnabled;
        }

        private void OnDisable()
        {
            if (m_agent == null) { return; }

            m_agent.navigationEnabled = false;
            m_agent.collisionEnabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


        }

        public override void Process( ref float3 position, ref float3 velocity, ref float3 lookAt )
        {
            if(m_agent == null) { return; }

            position = m_agent.pos;
            //m_agent.prefVelocity = velocity;
            //lookAt = m_agent.velocity;
            
            //Apply ORCA Agent position
            //update ORCA Agent prefVelocity
        }

    }
}