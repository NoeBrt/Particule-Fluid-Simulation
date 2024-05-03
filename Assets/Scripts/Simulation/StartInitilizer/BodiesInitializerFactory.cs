using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Simulation
{
    public class BodiesInitializerFactory
    {
        public static BodiesInitializer Create(SimulationParameter simulationParameter)
        {
            switch (simulationParameter.simulationType)
            {
                case SimulationType.Galaxy:
                    return new GalaxyInitializer(simulationParameter);
                default:
                    return new GalaxyInitializer(simulationParameter);
            }

        }
    }

}