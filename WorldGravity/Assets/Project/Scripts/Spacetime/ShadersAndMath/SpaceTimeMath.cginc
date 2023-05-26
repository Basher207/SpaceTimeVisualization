// Script for calculation space time height at any given point (proportional to gravitational acceleration potential)


#define MAX_PLANETS 15
float4 _planetsInfo[MAX_PLANETS];

float _ease;	
float PI = 3.14159265359;

float OrbitalPeriod (float distanceToAclCenter, float mass, float gravConstant)
{
	float deltaCubed = pow(distanceToAclCenter, 3);
	float mu = gravConstant * mass;
			
	return 2 * PI * sqrt(deltaCubed / mu);
}
float RotationRate (float distanceToAclCenter, float mass, float gravConstant)
{
	return 2 * PI / OrbitalPeriod(distanceToAclCenter, mass, gravConstant);
}
float CentrifugalForce (float distanceToAclCenter, float massSun, float massPlanet, float gravConstant)
{
	float rotationRate = RotationRate(distanceToAclCenter, massSun, gravConstant);
	float centForce = pow(rotationRate, 2) * distanceToAclCenter * massPlanet;
	return centForce;
}
float CentrifugalAcl (float rotationRate, float distanceToPoint, float massSun, float gravConstant)
{
	float centForce = pow(rotationRate, 2) * distanceToPoint;
	return centForce;
}
float AngularMomentum (float mass, float rotationRate, float distanceToAclCenter)
{
	float aclMom = rotationRate * mass * pow(distanceToAclCenter, 2);
	return aclMom;
}
float CentPotential (float angularMomentum, float distanceToAclCenter, float otherMass, float gravConstant)
{
	float centPotential = pow(angularMomentum, 2) / (2 * otherMass * pow(distanceToAclCenter, 2));
	return centPotential;
}
float OrbitSpeed (float distance, float acl) {
	float speed = sqrt (1 / (acl * distance));
	return speed;
}
float GetYPosAtPoint (float2 pos, float withCentPotential)
{
	float yPos = 0;
	
	float3 centerOfMass = ((_planetsInfo[0].xyz * _planetsInfo[0].w) + (_planetsInfo[1].xyz * _planetsInfo[1].w)) / (_planetsInfo[0].w + _planetsInfo[1].w);

	centerOfMass.y = 0;
	
	float distanceToCenterFromPerspectiveOfP1 = length(centerOfMass.xz - _planetsInfo[1].xz);
	float distanceToPoint = length(centerOfMass.xz - pos);
	
	float massOfP0 = _planetsInfo[0].w;
	float massOfP1 = _planetsInfo[1].w;
	
	float gravConstant = 6.674e-11f;
	
	float rotationRate = RotationRate(distanceToCenterFromPerspectiveOfP1, massOfP0, gravConstant);
	float angularMomentum = AngularMomentum(1, rotationRate, distanceToPoint);
	
	float centPotential = CentPotential(angularMomentum, distanceToPoint, 1, gravConstant);

	for (int i = 0; i < MAX_PLANETS; i++)
	{
		float distance = max(0.01, length (pos - _planetsInfo[i].xz));
		yPos -= gravConstant * (_planetsInfo[i].w) / distance;
	}
	
	yPos += -centPotential * withCentPotential;
	yPos *= _ease;
	
	return yPos;
}