﻿// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com
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

namespace Nebukam.Beacon.ORCA
{
    public class ORCABundle<AgentType> : System.IDisposable
        where AgentType : Agent, IAgent, new()
    {

        protected AgentGroup<AgentType> m_agents = new AgentGroup<AgentType>();
        protected ObstacleGroup m_staticObstacles = new ObstacleGroup();
        protected ObstacleGroup m_dynamicObstacles = new ObstacleGroup();
        protected Nebukam.ORCA.ORCA m_orca;

        public AgentGroup<AgentType> agent { get { return m_agents; } }
        public ObstacleGroup staticObstacles { get { return m_staticObstacles; } }
        public ObstacleGroup dynamicObstacles { get { return m_dynamicObstacles; } }
        public Nebukam.ORCA.ORCA orca { get { return m_orca; } }

        public ORCABundle()
        {
            m_orca = new Nebukam.ORCA.ORCA();

            m_orca.agents = m_agents;
            m_orca.staticObstacles = m_staticObstacles;
            m_orca.dynamicObstacles = m_dynamicObstacles;
        }

        public AgentType NewAgent(float3 position)
        {
            return m_agents.Add(position) as AgentType;
        }

        #region System.IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) { return; }
            
            m_agents.Clear(true);
            m_staticObstacles.Clear(true);
            m_dynamicObstacles.Clear(true);
            m_orca.DisposeAll();

        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            System.GC.SuppressFinalize(this);
        }

        #endregion

    }
}
