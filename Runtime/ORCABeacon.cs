using UnityEngine;
using Nebukam.ORCA;

namespace Nebukam.Beacon.ORCA
{
    public class ORCABeacon : BeaconModule<ORCABeacon>
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RuntimeInitializeOnLoad() { StaticInitialize(); }

        protected ORCABundle<Agent> m_defaultBundle;


        protected override void Init()
        {
            m_defaultBundle = Pooling.Pool.Rent<ORCABundle<Agent>>();
            m_processes.Add(m_defaultBundle.orca);
        }
        
    }
}
