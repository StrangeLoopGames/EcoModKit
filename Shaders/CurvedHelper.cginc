#ifndef CURVEDHELPER
#define CURVEDHELPER

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

// This is where the curvature is applied
float4 curveVertex(float4 vertex, float4x4 Object2World, float4x4 World2Object)
{
#ifdef NO_CURVE
	return vertex;
#else
	float4 vv = mul(Object2World, vertex);

	float3 toVertex = vv.xyz - _WorldCenter;

	float3 v1 = float3(0, toVertex.y, 0);			// first basis vector, height of the vertex
	float3 xz = float3(toVertex.x, 0, toVertex.z);
	float3 v2 = normalize(xz) * abs(toVertex.y);	// second basis vector, the xz- position, of the same height

	float d = length(toVertex.xz);
	float c = 2 * 3.14159 * abs(_WorldRadius); // circumference of the world

	// if we slerp from v1->v2, we get the curvature we want
	float t = clamp(d / (c / 4), 0, 2);
	float3 s = slerp(v1, v2, t);
	float3 expected = _WorldCenter + s;

	// w is *very* important!
	vv.xyz = expected;
	return mul(World2Object, vv);
#endif
}

float4 curveVertexFixed(float4 vertex, float4x4 Object2World, float4x4 World2Object, float radius, float3 center)
{
    float4 vv = mul(Object2World, vertex);

	float3 toVertex = vv.xyz - center;

	float3 v1 = float3(0, toVertex.y, 0);			// first basis vector, height of the vertex
	float3 xz = float3(toVertex.x, 0, toVertex.z);
	float3 v2 = normalize(xz) * abs(toVertex.y);			// second basis vector, the xz- position, of the same height

	float d = length(toVertex.xz);
	float c = 2 * 3.14159 * abs(radius); // circumference of the world

	// if we slerp from v1->v2, we get the curvature we want
	float t = clamp(d / (c / 4), 0, 2);
	float3 s = slerp(v1, v2, t);
	float3 expected = center + s;

	// w is *very* important!
	vv.xyz = expected;
	return mul(World2Object, vv);
}

float4 curveVertexFixed(float4 vertex, float radius, float3 center)
{
    //float4 vv = mul(Object2World, vertex);
    float4 vv = vertex;

    float3 toVertex = vv.xyz - center;

    float3 v1 = float3(0, toVertex.y, 0);			// first basis vector, height of the vertex
    float3 xz = float3(toVertex.x, 0, toVertex.z);
    float3 v2 = normalize(xz) * abs(toVertex.y);	// second basis vector, the xz- position, of the same height

    float d = length(toVertex.xz);
    float c = 2 * 3.14159 * abs(radius); // circumference of the world

                                         // if we slerp from v1->v2, we get the curvature we want
    float t = clamp(d / (c / 4), 0, 2);
    float3 s = slerp(v1, v2, t);
    float3 expected = center + s;

    // w is *very* important!
    vv.xyz = expected;
    //return mul(World2Object, vv);
    return vv;
}

#endif
