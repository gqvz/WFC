namespace WFC;

public static class IEnumerableExtensions
{
	public static T Random<T>(this T[] source)
	{
		return source[System.Random.Shared.Next(source.Length)];
	}
}