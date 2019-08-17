using UnityEngine;
using Nebukam.ORCA;

namespace Nebukam.Beacon.ORCA
{
    public class ORCABeacon : BeaconModule<ORCABeacon>
    {
        
        protected ORCABundle<Agent> m_defaultBundle;
        public ORCABundle<Agent> defaultBundle { get { return m_defaultBundle; } }

        protected override void Init()
        {
            m_defaultBundle = new ORCABundle<Agent>();
            m_processes.Add(m_defaultBundle.orca);
        }

    }
}
