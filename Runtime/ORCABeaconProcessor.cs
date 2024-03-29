﻿// Copyright (c) 2021 Timothé Lapetite - nebukam@gmail.com
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

using Nebukam.ORCA;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Common;

namespace Nebukam.Beacon.ORCA
{
    [AddComponentMenu("Nebukam/Beacon/ORCA/ORCA Beacon Processor")]
    public class ORCABeaconProcessor : BeaconProcessor
    {

        [Header("Settings")]
        [Tooltip("When true, the component will create and manage the agent itself and register it to the default bundle.\n" +
            "Otherwise, you are responsible for providing this component with an Agent.")]
        public bool selfManaged = true;
        [Tooltip("Always update Agent. Use this is you intend to modify agent's settings often.")]
        public bool alwaysUpdate = false;

        [Header("Template")]
        [Tooltip("Agent's collision radius [0,10]"), Range(0f, 10f)]
        public float radius = 0.5f;
        [Tooltip("Agent's collision radius toward obstacles [0,10]"), Range(0f, 10f)]
        public float radiusObst = 0.5f;
        [Tooltip("Agent's height [0.001,10]"), Range(0.001f, 10f)]
        public float height = 1.0f;

        [Header("Limits & Ranges")]
        [Tooltip("Maximum number of agents from its closest surroundings that will be accounted for. Higher numbers means slower simulation."), Range(0, 50)]
        public int maxNeighbors = 15;
        [Tooltip("Maximum distance that agent will probe for other agents. Higher numbers means slower simulation. [0, 100]"), Range(0f, 100f)]
        public float neighborDist = 20.0f;

        [Tooltip("Higher numbers means slower simulation [0f, 100f]"), Range(0f, 100f)]
        public float timeHorizon = 15.0f;
        [Tooltip("Higher numbers means slower simulation [0f, 5f]"), Range(0f, 5f)]
        public float timeHorizonObst = 1.2f;

        [Header("Collision")]
        [Tooltip("Layers this agent 'exists' on")]
        public ORCALayer layerOccupation = ORCALayer.ANY;
        [Tooltip("Layers this agent ignores")]
        public ORCALayer layerIgnore = ORCALayer.NONE;
        [Tooltip("Whether this agent's navigation is processed")]
        public bool navigationEnabled = true;
        [Tooltip("Whether this agent's is considered at all by other agents")]
        public bool collisionEnabled = true;

        protected Nebukam.ORCA.Agent m_agent = null;
        public Nebukam.ORCA.Agent agent
        {
            get { return m_agent; }
            set
            {
                if (m_agent == value)
                    return;

                if (selfManaged && m_agent != null)
                    m_agent.Release();

                m_agent = value;

                if (m_agent != null)
                {
                    m_agent.pos = transform.position;
                    UpdateAgentSettings();
                }
            }
        }

        private void Awake()
        {

        }

        public void UpdateAgentSettings()
        {
            m_agent.radius = radius;
            m_agent.radiusObst = radiusObst;
            m_agent.maxSpeed = controller.speed;
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

            if (selfManaged)
                agent = ORCABeacon.Get.defaultBundle.NewAgent(transform.position);

            if (m_agent == null)
                return;

            m_agent.pos = transform.position;
            m_agent.navigationEnabled = navigationEnabled;
            m_agent.collisionEnabled = collisionEnabled;
        }

        private void OnDisable()
        {
            if (selfManaged)
                agent = null;

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

        public override void Apply(
            ref float3 position,
            ref float3 velocity,
            ref float3 heading,
            float sqDistToGoal)
        {
            if (m_agent == null) { return; }

            if (alwaysUpdate)
                UpdateAgentSettings();

            if (!m_agent.navigationEnabled)
            {
                m_agent.pos = position;
                m_agent.prefVelocity = float3(0f);
                m_agent.velocity = float3(0f);
                return;
            }

            //Ensure baseline is up-to-date
            float3 aPos = m_agent.pos;
            if (controller.plane == AxisPair.XY)
                aPos.z = position.z;
            else
                aPos.y = position.y;

            //Feed back position & prefVelocity to agent, ensuring elevation is updated
            m_agent.pos = aPos;
            m_agent.prefVelocity = velocity;


            position = aPos;
            heading = m_agent.velocity;
            velocity = m_agent.velocity;

        }

        private void OnDestroy()
        {
            agent = null;
        }

    }
}