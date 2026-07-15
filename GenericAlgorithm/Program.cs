using GenericAlgorithm;

Console.WriteLine("Hello, World!");

List<char> numbers = ['f','a', 'd', 'c', 'b', 'u','q', 'n', 'k'];
var sortedChars= GenericInsertionSort.Sort(numbers);

List<string> result = GenericInsertionSort.Sort<string>(["@hello", "#ilovepeople#", "&banana", "*ben", "peop^le"]);
