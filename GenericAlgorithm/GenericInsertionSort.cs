namespace GenericAlgorithm;

/// <summary>
/// A  class that implements a generic insertion sort algorithm
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class GenericInsertionSort
{
    /// <summary>
    /// A method that sorts a list of objects of type T in ascending order<br/>
    /// requires: a list of object T that implements the IComparable interface<br/>
    /// modifies: items<br/>
    /// effect: rearranges the list of object T in ascending order
    /// </summary>
    /// <param name="items">List of objects of type T</param>
    /// <typeparam name="T">The type</typeparam>
    /// <returns>Sorted list in ascending order</returns>
    public static List<T> Sort<T>(List<T> items)
        where T : IComparable<T>
    {
        for (int j = 1; j < items.Count; j++)
        {
            var key = items[j];
            int i = j - 1;

            while (i > -1)
            {
                var item = items[i];
                if (item.CompareTo(key) > 0)
                {
                    items[i + 1] = items[i];
                    i -= 1;
                }
                else
                {
                    break;
                }
            }

            items[i + 1] = key;
        }

        return items;
    }
}