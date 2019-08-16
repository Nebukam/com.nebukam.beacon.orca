using Nebukam.ORCA;

namespace Nebukam.Beacon.ORCA
{
    public class ORCABundle<AgentType> : Pooling.PoolItemEx
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

        public override void Init()
        {

        }

        protected override void CleanUp()
        {
            m_agents.Clear(true);
            m_staticObstacles.Clear(true);
            m_dynamicObstacles.Clear(true);
        }

    }
}
