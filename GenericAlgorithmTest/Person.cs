namespace GenericAlgorithmTest;

/// <summary>
/// A custom type used to test how <see cref="GenericAlgorithm.GenericInsertionSort"/> handles objects
/// </summary>
public class Person(string name, int age) : IComparable<Person>
{
    public string Name { get; } = name;
    public int Age { get; } = age;

    public int CompareTo(Person? other)
    {
        if (other is null) return 1;
        return Age.CompareTo(other.Age);
    }

    public override string ToString() => $"{Name}({Age})";
}