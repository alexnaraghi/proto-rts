using UnityEngine;

/// <summary>
/// Wrapper for UnityEngine.Random, for deterministic seeded values.
/// </summary>
public static class RtsRandom 
{
    public static int Seed
	{
		get
		{
            return Random.seed;
        }
		set
		{
            Random.seed = value;
        }
	}
	
	public static float Value
	{
		get
		{
            return Random.value;
        }
	}
	
	public static Vector3 InsideUnitCircle
	{
		get
		{
            return Random.insideUnitCircle;
        }
	}
	
	public static Vector3 InsideUnitSphere
	{
		get
		{
            return Random.insideUnitSphere;
        }
	}
	
	public static Vector3 OnUnitSphere
	{
		get
		{
            return Random.onUnitSphere;
        }
	}
	
	public static Quaternion Rotation
	{
		get
		{
            return Random.rotation;
        }
	}
	
	public static Quaternion RotationUniform
	{
		get
		{
            return Random.rotationUniform;
        }
	}
	
	public static float Range(float min, float max)
	{
        return Random.Range(min, max);
    }
	
	public static Color ColorHSV()
	{
        return Random.ColorHSV();
    }
}
