using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{

public class SimulationParameter : MonoBehaviour
{
    public enum SimulationType
    {
        Galaxy,
        Universe,
        Collision
    }
    private int bodiesCount;

    private float radius;
    private float thickness; //utile seulement pour le type galaxies

    private float initialVelocity;

    private float smoothingLength;

    private float blackHoleMass;

    private float interactionRate;
    private float timeStep;


    public float Radius
    {
        get { return radius; }
        set
        {
            radius = value;
            GlobalManager.Instance.UIManager.RadiusSlider.value = value;
        }
    }

    public float Thickness
    {
        get { return thickness; }
        set
        {
            thickness = value;
            GlobalManager.Instance.UIManager.ThicknessSlider.value = value;
        }
    }


    public float InitialVelocity
    {
        get { return initialVelocity; }
        set
        {
            initialVelocity = value;
            GlobalManager.Instance.UIManager.InitialVelocitySlider.value = value;
        }
    }

    public float SmoothingLength
    {
        get { return smoothingLength; }
        set
        {
            smoothingLength = value;
            GlobalManager.Instance.UIManager.SmoothingLengthSlider.value = value;
        }
    }

    public float BlackHoleMass
    {
        get { return blackHoleMass; }
        set
        {
            blackHoleMass = value;
            GlobalManager.Instance.UIManager.BlackHoleMassSlider.value = value;
        }
    }

    public float InteractionRate
    {
        get { return interactionRate; }
        set
        {
            interactionRate = value;
            GlobalManager.Instance.UIManager.InteractionRateSlider.value = value;
        }
    }

    public float BodiesCount
    {
        get { return (int) bodiesCount; }
        set
        {
            bodiesCount = (int)value;
            GlobalManager.Instance.UIManager.BodiesCountSlider.value = bodiesCount;
        }
    }
    public float TimeStep
    {
        get { return timeStep; }
        set
        {
            timeStep = value;
            GlobalManager.Instance.UIManager.TimeStepSlider.value = value;
        }
    }


    public SimulationType simulationType { get; set; }


    public void Init()
    {
        BodiesCount = GlobalManager.Instance.UIManager.BodiesCountSlider.value;
        Radius = GlobalManager.Instance.UIManager.RadiusSlider.value;
        Thickness = GlobalManager.Instance.UIManager.ThicknessSlider.value;
        InitialVelocity = GlobalManager.Instance.UIManager.InitialVelocitySlider.value;
        SmoothingLength = GlobalManager.Instance.UIManager.SmoothingLengthSlider.value;
        BlackHoleMass = GlobalManager.Instance.UIManager.BlackHoleMassSlider.value;
        InteractionRate = GlobalManager.Instance.UIManager.InteractionRateSlider.value;
        // TimeStep= GlobalManager.Instance.UIManager.TimeStepSlider.value;
        simulationType = GlobalManager.Instance.UIManager.TypeDropdown.value == 0 ? SimulationType.Galaxy : GlobalManager.Instance.UIManager.TypeDropdown.value == 1 ? SimulationType.Universe : SimulationType.Collision;
    }


    public void setDropDownType()
    {
        simulationType = GlobalManager.Instance.UIManager.TypeDropdown.value == 0 ? SimulationType.Galaxy : GlobalManager.Instance.UIManager.TypeDropdown.value == 1 ? SimulationType.Collision : SimulationType.Universe;

    }


    
}

}
