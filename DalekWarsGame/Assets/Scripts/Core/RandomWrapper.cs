//wrap around Unity Random or Custom Random for use outside of Unity
public static class RandomWrapper
{
#if AITRAINING
    private static System.Random nonUnityRandom;
#endif

    // Start is called before the first frame update
    public static float Range(float min, float max)
    {
#if AITRAINING
        if (nonUnityRandom == null)
            nonUnityRandom = new System.Random();

        return min + ((max - min) * (float)nonUnityRandom.NextDouble());
#else
         return UnityEngine.Random.Range(min, max);
#endif
    }

    public static int Range(int min, int max)
    {
#if AITRAINING
        if (nonUnityRandom == null)
            nonUnityRandom = new System.Random();

        return nonUnityRandom.Next(min, max);
#else
        return UnityEngine.Random.Range(min, max);
#endif

    }
}
