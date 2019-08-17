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
