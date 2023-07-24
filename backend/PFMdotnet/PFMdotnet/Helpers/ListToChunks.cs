namespace PFMdotnet.Helpers
{
    public class ListToChunks
    {
        public static IEnumerable<List<T>> ChunkList<T>(List<T> list, int chunkSize)
        {
            for (int i = 0; i < list.Count; i += chunkSize)
            {
                yield return list.GetRange(i, Math.Min(chunkSize, list.Count - i));
            }
        }

    }
}
