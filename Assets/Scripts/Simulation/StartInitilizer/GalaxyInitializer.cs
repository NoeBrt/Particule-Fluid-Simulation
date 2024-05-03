using System.Threading.Tasks;
using UnityEngine;
using Simulation.Struct;
namespace Simulation
{
    public class GalaxyInitializer : BodiesInitializer
    {
        float thickness;
        float diameter;
        float centerConcentration;


        public GalaxyInitializer(SimulationParameter simulationParameter) : base(simulationParameter)
        {
            this.thickness = simulationParameter.Thickness;
            this.diameter = simulationParameter.Radius;

        }


        public override Particule[] InitStars()
        {
            var data = new Particule[base.bodiesCount];
            for (int i = 0; i < base.bodiesCount; i++)
            {
                var star = new Particule();
                star.position =InsideRectangle(base.border);
                star.velocity = RandomVelocity(base.initialVelocity);
                Debug.Log(star.position);
                star.radius=20f;
                data[i] = star;
            }
            return data;
        }


        private Vector2 InsideRectangle(Vector2 border)
        {
            float x = Random.Range(-border.x / 2, border.x / 2);
            float y = Random.Range(-border.y / 2, border.y / 2);
            return new Vector2(x, y);
        }
        private Vector2 RandomVelocity(float initialVelocity)
        {
            float x = Random.Range(-initialVelocity, initialVelocity);
            float y = Random.Range(-initialVelocity, initialVelocity);
            return new Vector2(x, y).normalized;
        }
    
    }
}