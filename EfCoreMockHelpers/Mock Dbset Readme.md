
### Usage Example for CreateAsyncDbSet Method to mock a dbset with async capabilities
1. Define Your Entities

```
public class SampleSource
{
    public int SampleSourceId { get; set; }
    public string SampleSourceName { get; set; }
    public string SampleSourceType { get; set; }
}
```

2. Mock the DbContext and DbSets

```
// Arrange
// Create a sample entity
var sampleSource = new SampleSource 
{ 
    SampleSourceId = 1, 
    SampleSourceName = "SourceA",
    SampleSourceType = "Non-Integrated" 
};
````
```
// Mock the DbSet<SampleSource> with the sample data
var mockSamples = MockDbSet.CreateAsyncDbSet(new List<SampleSource> { sampleSource });

// Create a mock DbContext
var _dbContextMock = new Mock<YourDbContext>(); // Replace with your actual DbContext type

// Configure the DbContext to return the mock DbSet
_dbContextMock
    .Setup(m => m.SampleSources)
    .Returns(mockSamples.Object);

```
