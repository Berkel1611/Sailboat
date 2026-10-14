namespace Sailboat.Simulation
{
    /// <summary>
    /// Physical description of a single foil (sail, keel or rudder) used by <see cref="FoilMath"/>
    /// </summary>
    public readonly struct FoilParameters
    {
        public FoilParameters(float area, float fluidDensity, float liftScale, float dragScale, float parasiticDrag)
        {
            Area = area;
            FluidDensity = fluidDensity;
            LiftScale = liftScale;
            DragScale = dragScale;
            ParasiticDrag = parasiticDrag;
        }

        /// <summary>
        /// Effective area in square meters. For the sail it is the full area multiplied by the opening.
        /// </summary>
        public float Area { get; }

        /// <summary>
        /// Density of the fluid flowing around the foil in kg/m3 (air for the sail, water for the keel).
        /// </summary>
        public float FluidDensity { get; }

        /// <summary>
        /// Multiplier for the lift coefficient.
        /// </summary>
        public float LiftScale { get; }

        /// <summary>
        /// Multiplier for the drag coefficient.
        /// </summary>
        public float DragScale { get; }

        /// <summary>
        /// Constant drag coefficient added regardless of the angle attack.
        /// </summary>
        public float ParasiticDrag { get; }
    }
}
