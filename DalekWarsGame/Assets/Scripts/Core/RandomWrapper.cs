//wrap around Unity Random or Custom Random for use outside of Unity
public static class RandomWrapper
{
    // Start is called before the first frame update
    public static float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
