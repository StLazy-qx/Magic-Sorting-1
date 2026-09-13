namespace Assets.Source.Scripts.Extensions
{
    public static class ShuffleExtensions
    {
        public static void FisherYatesShuffle<T>(
            this T[] array, 
            System.Random randomGenerator = null)
        {
            randomGenerator ??= new System.Random();

            for (int index = array.Length - 1; index > 0; index--)
            {
                int randomIndex = randomGenerator.Next(index + 1);
                (array[index], array[randomIndex]) = (array[randomIndex], array[index]);
            }
        }
    }
}
