using GenericAlgorithm;

Console.WriteLine("Hello, World!");

List<char> numbers = ['f','a', 'd', 'c', 'b', 'u','q', 'n', 'k'];
var sortedChars= GenericInsertionSort.Sort(numbers);

foreach (var sortedChar in sortedChars)
{
    Console.WriteLine(sortedChar);
}