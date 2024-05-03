using System.Collections;
using UnityEngine;

namespace Simulation
{
    public class GalaxySimulationGPU : MonoBehaviour
    {
        bool simulationStarted = false;
        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private ComputeBuffer starsBuffer;
        bool render = false;
        private int kernel;
        int starCount;
        SimulationParameter simulationParameter;

        private Bounds bounds;
        public Mesh mesh;
        public Shader shader;
        Material material;

        public ComputeBuffer argsBuffer;
        Texture2D gradientTexture;

        private void Start()
        {

        }



        public static void TextureFromGradient(ref Texture2D texture, int width, FilterMode filterMode = FilterMode.Bilinear)
        {
            if (texture == null)
            {
                texture = new Texture2D(width, 1);
            }
            else if (texture.width != width)
            {
                texture.Reinitialize(width, 1);
            }
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = filterMode;

            texture.Apply();
        }

        public static ComputeBuffer CreateArgsBuffer(Mesh mesh, int numInstances)
        {
            const int subMeshIndex = 0;
            uint[] args = new uint[5];
            args[0] = (uint)mesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)numInstances;
            args[2] = (uint)mesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)mesh.GetBaseVertex(subMeshIndex);
            args[4] = 0; // offset

            ComputeBuffer argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            argsBuffer.SetData(args);
            return argsBuffer;
        }
        // This method is called to start the simulation
        public void Spawn()
        {
            Delete(); // Clean up any existing resources

            simulationParameter = GlobalManager.Instance.SimulationParameter;

            starCount = (int)simulationParameter.BodiesCount;

            SetupShader(starCount);
            InitStarsAttribute(starCount, simulationParameter.TimeStep, simulationParameter.SmoothingLength,
                simulationParameter.InteractionRate, simulationParameter.BlackHoleMass);
            InitStarsPos(simulationParameter);

            material = new Material(shader);
            material.SetBuffer("particuleBuffer", starsBuffer);
            bounds = new Bounds(Vector3.zero, Vector3.one * 10000);
            argsBuffer = CreateArgsBuffer(mesh, starCount);

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

        void LateUpdate()
        {
            if (simulationStarted)
            {
                TextureFromGradient(ref gradientTexture, 256);
                material.SetTexture("ColourMap", gradientTexture);
                Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);


            }
        }

        void RunComputeShader()
        {
            computeShader.Dispatch(kernel, starCount / 128 + 1, 1, 1); // Dispatch the compute shader
        }

        void UpdateDynamicParameter(float smoothingLength, float interactionRate, float blackHoleMass, float timeStep)
        {
            computeShader.SetFloat("deltaTime", timeStep); // Update the time step dynamically
        }

        public void SetupShader(int n)
        {
            kernel = computeShader.FindKernel("UpdateParticules");
            if (kernel == -1)
            {
                Debug.LogError("Kernel 'UpdateParticles' not found in the compute shader.");
                return;
            }

            int bufferStride = sizeof(float) * 5; // Assuming each particle has 5 floats
            starsBuffer = new ComputeBuffer(starCount, bufferStride);
            computeShader.SetBuffer(kernel, "particules", starsBuffer);
        }

        public void InitStarsAttribute(int n, float deltaTime, float smoothingLength, float interactionRate, float blackHoleMass)
        {
            computeShader.SetInt("particleCount", n);
            computeShader.SetFloat("deltaTime", deltaTime);
        }

        public void InitStarsPos(SimulationParameter parameter)
        {
            starsBuffer.SetData(BodiesInitializerFactory.Create(parameter).InitStars());
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
