# SpaceTimeVisualization
Visualize space-time in Unity with this project. Interact with different modes and features using the following keys:

**Tab:** Switch between stationary and rotational reference frames.
**Left Mouse Button (Hold):** Look around in the scene.
**Space:** Toggle overview mode.
**R:** Restart the visualization.

Areas of the visualization are color-coded and shaped according to acceleration:

**Blue/Steep:** Indicates high acceleration.
**Red/Flat:** Signifies low acceleration.

When in the rotational reference frame mode (activated with Tab), the y position corresponds to acceleration relative to the system's largest planet.

This project is designed as an educational tool for explaining Lagrange points, though it currently lacks a comprehensive explanation.

# Technical Details

The visualization utilizes a high-density quad with vertices' height proportional to the acceleration potential of an object at a point, achieved using a vertex shader. The color is determined through a fragment shader.