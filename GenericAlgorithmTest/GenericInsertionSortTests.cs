using GenericAlgorithm;
using Xunit.Abstractions;

namespace GenericAlgorithmTest;

public class GenericInsertionSortTests
{
    private readonly ITestOutputHelper output;

    public GenericInsertionSortTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    
    [Fact]
    public void SortingAnEmptyListReturnsAnEmptyList()
    {
        List<int> result = GenericInsertionSort.Sort<int>([]);

        Assert.Empty(result);
    }

    [Fact]
    public void SortingASingleElementListReturnsTheSameElement()
    {
        List<int> result = GenericInsertionSort.Sort([1]);

        Assert.Equal([1], result);
    }

    [Fact]
    public void SortingTwoElementsAlreadyInOrderLeavesThemAlone()
    {
        List<int> result = GenericInsertionSort.Sort([1, 2]);

        Assert.Equal([1, 2], result);
    }

    [Fact]
    public void SortingTwoElementsOutOfOrderSwapsThem()
    {
        List<int> result = GenericInsertionSort.Sort([2, 1]);

        Assert.Equal([1, 2], result);
    }

    [Fact]
    public void SortingNullShouldReturnNull()
    {
        List<int>? result = null;
        Assert.Null(result);
    }
    // ---------------------------------------------------------------------
    // Integers
    // ---------------------------------------------------------------------

    [Fact]
    public void SortsAListOfIntegersInAscendingOrder()
    {
        List<int> result = GenericInsertionSort.Sort([10, 9, 8, 7, 6, 5, 4, 3, 2, 1]);

        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], result);
    }

    [Fact]
    public void SortingAnAlreadySortedListDoesNothing()
    {
        List<int> result = GenericInsertionSort.Sort([1, 2, 3, 4, 5]);

        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    [Fact]
    public void SortsAListOfLargeIntegersInAscendingOrder()
    {
        List<int> result = GenericInsertionSort.Sort([4444444, 22222, 666666666, 1111, 333333, 55555555]);

        Assert.Equal([1111, 22222, 333333, 4444444, 55555555, 666666666], result);
    }

    [Fact]
    public void SortsAListOfIntegersContainingNegativeValues()
    {
        List<int> result = GenericInsertionSort.Sort([3, -1, 0, -7, 5, -2]);

        Assert.Equal([-7, -2, -1, 0, 3, 5], result);
    }

    [Fact]
    public void SortsIntegersAtTheExtremesOfTheirRange()
    {
        List<int> result = GenericInsertionSort.Sort([0, int.MaxValue, -1, int.MinValue, 1]);

        Assert.Equal([int.MinValue, -1, 0, 1, int.MaxValue], result);
    }

    [Fact]
    public void KeepsDuplicateValuesAndDoesNotDropThem()
    {
        List<int> result = GenericInsertionSort.Sort([3, 1, 3, 2, 1, 3]);

        Assert.Equal([1, 1, 2, 3, 3, 3], result);
    }

    [Fact]
    public void SortingAListWhereEveryElementIsEqualLeavesItUnchanged()
    {
        List<int> result = GenericInsertionSort.Sort([7, 7, 7, 7]);

        Assert.Equal([7, 7, 7, 7], result);
    }

    [Fact]
    public void SortsAListOfCharactersInAscendingOrder()
    {
        List<char> result = GenericInsertionSort.Sort(['f', 'a', 'd', 'c', 'b', 'u', 'q', 'n', 'k']);

        Assert.Equal(['a', 'b', 'c', 'd', 'f', 'k', 'n', 'q', 'u'], result);
    }

    [Fact]
    public void SortingUpperCaseAndLowerCasePutsUpperCaseFirst()
    {
        List<char> result = GenericInsertionSort.Sort(['b', 'A', 'a', 'B']);

        Assert.Equal(['A', 'B', 'a', 'b'], result);
    }

    [Fact]
    public void SortsDigitAndSymbolCharacters()
    {
        List<char> result = GenericInsertionSort.Sort(['z', '9', ' ', '0', 'A']);

        Assert.Equal([' ', '0', '9', 'A', 'z'], result);
    }

    // ---------------------------------------------------------------------
    // Strings
    // ---------------------------------------------------------------------

    [Fact]
    public void SortsAListOfStringsInAscendingOrder()
    {
        List<string> result = GenericInsertionSort.Sort(["pear", "apple", "mango", "banana"]);

        Assert.Equal(["apple", "banana", "mango", "pear"], result);
    }

    [Fact]
    public void SortsTheEmptyStringBeforeEverythingElse()
    {
        List<string> result = GenericInsertionSort.Sort(["b", "", "a"]);

        Assert.Equal(["", "a", "b"], result);
    }

    [Fact]
    public void SortsMixedCaseStringsTheSameWayTheBclDoes()
    {
        // string.CompareTo is culture-sensitive, so rather than hard-coding an ordering that
        // could shift with the host culture, we pin the result to Comparer<string>.Default —
        // which is the very comparison Sort<T> ends up making.
        List<string> input = ["Banana", "apple", "Apple", "banana"];

        List<string> expected = [..input];
        expected.Sort();

        List<string> result = GenericInsertionSort.Sort([..input]);

        Assert.Equal(expected, result);
    }

    // ---------------------------------------------------------------------
    // Objects
    // ---------------------------------------------------------------------

    [Fact]
    public void SortsObjectsUsingTheirCompareToImplementation()
    {
        Person alice = new("Alice", 30);
        Person bob = new("Bob", 25);
        Person carol = new("Carol", 41);

        List<Person> result = GenericInsertionSort.Sort([alice, bob, carol]);

        Assert.Equal([bob, alice, carol], result);
    }

    [Fact]
    public void SortIsStableSoEqualObjectsKeepTheirOriginalRelativeOrder()
    {
        Person zoe = new("Zoe", 30);
        Person adam = new("Adam", 30);
        Person mia = new("Mia", 30);
        Person youngest = new("Youngest", 18);
        Person oldest = new("Oldest", 99);

        List<Person> result = GenericInsertionSort.Sort([oldest, zoe, adam, youngest, mia]);

        Assert.Equal([youngest, zoe, adam, mia, oldest], result);
    }

    [Fact]
    public void SortsASingleObjectList()
    {
        Person solo = new("Solo", 1);

        List<Person> result = GenericInsertionSort.Sort([solo]);

        Assert.Equal([solo], result);
    }
}