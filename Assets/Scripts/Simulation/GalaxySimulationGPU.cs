using System.Collections;
using UnityEngine;

namespace Simulation
{
    public class GalaxySimulationGPU : MonoBehaviour
    {
        bool simulationStarted = false;
        [SerializeField] ComputeShader computeShader;
        [SerializeField] private ComputeBuffer starsBuffer;
        [SerializeField] private Material renderMaterial;
        [SerializeField] private Texture2D circleTexture; // Ensure this is set in the Unity Editor
        bool render=false;
        private int kernel;
        int starCount;
        SimulationParameter simulationParameter;

        // Start is called before the first frame update
        public void Spawn()
        {
            Delete();

            simulationParameter = GlobalManager.Instance.SimulationParameter;

            starCount = (int)simulationParameter.BodiesCount;

            SetupShader(starCount);
            InitStarsAttribute(starCount, simulationParameter.TimeStep, simulationParameter.SmoothingLength, 
                simulationParameter.InteractionRate, simulationParameter.BlackHoleMass);
            InitStarsPos(simulationParameter);

            renderMaterial.SetColor("colorStart", simulationParameter.Color.colorStart);
            renderMaterial.SetColor("colorEnd", simulationParameter.Color.colorEnd);
            renderMaterial.SetFloat("divider", simulationParameter.Color.divider);
            renderMaterial.SetFloat("_Size", 1.0f);  // Adjust size as needed
            renderMaterial.SetTexture("_SpriteTex", circleTexture); // Set the texture to be used for rendering circles

            simulationStarted = true;
            render = true;
        }

        void Update()
        {
            if (simulationStarted)
            {
                RunComputeShader();
                UpdateDynamicParameter(simulationParameter.SmoothingLength, simulationParameter.InteractionRate,
                    simulationParameter.BlackHoleMass, simulationParameter.TimeStep);
            }
        }

        void RunComputeShader()
        {
            computeShader.Dispatch(kernel, starCount / 128 + 1, 1, 1);
        }

        void UpdateDynamicParameter(float smoothingLength, float interactionRate, float blackHoleMass, float timeStep)
        {
            computeShader.SetFloat("deltaTime", timeStep);
        }

        public void SetupShader(int n)
        {
            kernel = computeShader.FindKernel("UpdateParticules");
            if (kernel == -1)
            {
                Debug.LogError("Kernel 'UpdateParticules' not found in the compute shader.");
                return;
            }

            int bufferStride = sizeof(float) * 5; // Adjust according to your particle structure
            starsBuffer = new ComputeBuffer(starCount, bufferStride);
            computeShader.SetBuffer(kernel, "particules", starsBuffer);
        }

        public void InitStarsAttribute(int n, float deltaTime, float smoothingLength, float interactionRate, float blackHoleMass)
        {
            computeShader.SetInt("particuleCount", n);
            computeShader.SetFloat("deltaTime", deltaTime);
        }

        public void InitStarsPos(SimulationParameter parameter)
        {
            starsBuffer.SetData(BodiesInitializerFactory.Create(parameter).InitStars());
        }

        private void OnRenderObject()
        {
            if (render)
            {
                renderMaterial.SetBuffer("particules", starsBuffer);
                renderMaterial.SetPass(0);
                Graphics.DrawProceduralNow(MeshTopology.Quads, starCount * 4);
            }
        }

        public void Delete()
        {
            if (starsBuffer != null)
            {
                starsBuffer.Dispose();
                starsBuffer = null;
            }
            simulationStarted = false;
            render = false;
        }
    }
}
