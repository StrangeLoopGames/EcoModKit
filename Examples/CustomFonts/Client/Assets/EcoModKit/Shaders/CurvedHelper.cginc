#ifndef CURVEDHELPER
#define CURVEDHELPER

#define CURVED_VERTEX(v) v = curveVertex(v, unity_ObjectToWorld, unity_WorldToObject)

float _WorldRadius;
float4 _WorldCenter;

float3 slerp(float3 p0, float3 p1, float t)
{
	float cosHalfTheta = dot(p0, p1);

	float halfTheta = acos(cosHalfTheta);
	float sinHalfTheta = sqrt(1.0f - cosHalfTheta * cosHalfTheta);

	float ratioA = sin((1.0f - t) * halfTheta) / sinHalfTheta;
	float ratioB = sin(t * halfTheta) / sinHalfTheta;

	return p0 * ratioA + p1 * ratioB;
}

// Curve world space position
float4 curve(float4 worldPos, float radius, float3 center)
{
#if defined(NO_CURVE) || defined(MINIMAP_NO_CURVE)
	return worldPos;
#else
	float3 toVertex = worldPos.xyz - center;
	float d = length(toVertex.xz);

	// angle on a sphere = XZ distance / circumfrence = (d / 2 pi r) * 2 pi radians
	float angle = d / radius;
	float s, c;
	sincos(angle, s, c);

	// normalized vector rotated by angle from up towards XZ
	worldPos.xz = (s / d) * toVertex.xz;
	worldPos.y = c;

	// final position 
	worldPos.xyz = (worldPos.xyz * toVertex.y) + center;
	return worldPos;
#endif
}

// Un-curve world space position
float3 inverseCurve(float3 worldPos, float radius, float3 center)
{
#if defined(NO_CURVE) || defined(MINIMAP_NO_CURVE)
	return worldPos;
#else
	float3 spherePos = worldPos - center.xyz;

	worldPos.y = length(spherePos);
	float3 norm = spherePos / worldPos.y;

	float angle = acos(norm.y);

	float d = angle * radius;
	worldPos.xz = normalize(spherePos.xz) * d;

	worldPos += center;

	return worldPos;
#endif
}

// Curve object space position
float4 curve(float4 vertex, float4x4 Object2World, float4x4 World2Object, float radius, float3 center)
{
#if defined(NO_CURVE) || defined(MINIMAP_NO_CURVE)
	return vertex;
#else
	vertex = mul(Object2World, vertex);
	vertex = curve(vertex, radius, center);
	return mul(World2Object, vertex);
#endif
}

// Curve object space position
float4 curveVertex(float4 vertex, float4x4 Object2World, float4x4 World2Object)
{
	return curve(vertex, Object2World, World2Object, _WorldRadius, _WorldCenter.xyz);
}

// Curve object space position with specified radius and center
float4 curveVertexFixed(float4 vertex, float4x4 Object2World, float4x4 World2Object, float radius, float3 center)
{
	return curve(vertex, Object2World, World2Object, radius, center);
}

// Curve world space position with specified radius and center
float4 curveVertexFixed(float4 vertex, float radius, float3 center)
{
	return curve(vertex, radius, center);
}

// Curve a world position
float4 curveWorldPos(float4 vertex)
{
	return curve(vertex, _WorldRadius, _WorldCenter.xyz);
}

// Un-curve worldspace position
float3 inverseCurve(float3 vertex)
{
	return inverseCurve(vertex, _WorldRadius, _WorldCenter.xyz);
}

#endif
