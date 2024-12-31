namespace AsyncEnumerable.Spikes.SerializationDeserialization;

public class Customer(string firstName, string lastName) {
    public string FirstName { get; init; } = firstName;
    public string LastName { get; init; } = lastName;

    public string WholeName => $"{FirstName} {LastName}";
}

