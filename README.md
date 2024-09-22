# Preface
In this exercise I followed the following principles:
- Addressing requirements, but not adding to them.
- Clean, readable, maintainable, extendable code and solution structure.
- Testability and TDD (Test Driven Design/Development).
- SOLID

## My Assumptions
- A depthChart belongs to a team.

## Design
The design of the application is structured as a multi-project solution, <br>
following a layered architecture that separates concerns and across different domains and services.<br>
This design emphasizes the Single Responsibility Principle (SRP) by keeping domain models focused on encapsulating business logic

I decided to use Enum (`NflPosition`) to denote well-defined positions, avoiding magic strings and improving maintainability.<br>
Using string to denote positions will allow easy abuse of creating random positions that<br>
Exceptions and preconditions are handled and documentation is provided as library usage is expected.
The code adheres to the Dependency Inversion Principle (DIP), and unit tested at the different levels,
including edge cases such null and exceptional scenarios. This fosters flexibilty maintainability, and adherence to best practices in object-oriented design.

### GetBackups
Defined to return lists as described in the use cases to implement section.<br>
If output is needed, the overridden `ToString()` method can be used.
### RemovePlayerFromDepthChart
Use cases conflict in return type - "return that player" vs "An empty list should be returned".<br>
This can't be done in C#, and I made the decision to return the Player, or null in it's absence.
The happy path should return a Player, making this the preffered choice.

### Data structures and complexity
DepthChart is implemented using underlying Dictionary, with a random-access List as the dictionary value and a HashSet as another dictionary value.
The Dictionary provides amortized O(1) complexity for read, write, and remove operations.<br>
The List in C# is backed by an array, giving O(1) complexity for appending, except when resizing is needed. Inserts and removals are O(n),<br>
where n is the number of players in a position, which is capped by the team size in our use case.
The HashSet also offers O(1) complexity for add, remove, and lookup operations due to its hash-based implementation.

#### API complexity
<b>GetBackups(NflPosition position, Player player): </b><br>
O(n), where n is the number of players at the specified position.<br>
This assumes you iterate through all players to find backups relative to the provided player.<br>
<b>RemovePlayerFromDepthChart(NflPosition position, Player player):</b><br>
O(n), where n is the number of players at the given position.<br>
Removing a player typically requires a search through the list of players at that position.<br>
<b>GetFullDepthChart():</b><br>
O(m * n), where m is the number of positions and n is the average number of players per position. <br>
Iterating through all positions and players to print the full depth chart.<br>
<b>AddPlayerToDepthChart(NflPosition position, Player player, int? positionDepth)</b><br>:
O(n), where n is the number of players at the specified position.<br>
The player has to be added at a specific depth, which may require shifting elements in the list.

### Trade-offs
- Solution architecture for the current small-scope needs vs potential extentions and uses: <br>
I gave a direction as to how to expand, but kept matters simple for the problem at hand.
For instance, I didn't use interfaces - as there are no multiple implementations. 
The Player record can later become an interface or an abstract class.
It would make sense to create a general domain project for such entities.

### Possible Extentions
- A Team class can introduce validation to ensure players are assigned to the correct teams, checking for conflicts (e.g., preventing duplicate player numbers across teams).
- We could add position-specific constraints, limiting players to valid positions based on their roles, enforcing rules through domain logic.
- Strings for External Input: Using strings for positions as external input introduces flexibility but comes with the risk of invalid data. Enum constraints can be enforced afterward to validate inputs, ensuring that only valid positions are used.
- Other Sports: At present, the design is focused on the NFL, but it can easily extend to other sports by adding enums for positions specific to each sport, along with depth charts tailored to each sport.

## Prerequisites

.NET 6.0 SDK or later

# Getting Started (Running the solution)

1. Clone the repository:
```git@github.com:veredv/FanDuel.SportsDepthChart.git```
cd NFLDepthChart

2. Build the solution:
dotnet build

3. Run the tests:
dotnet test

4. Run the application:
dotnet run --project src/NFL.DepthChart.CLI/NFL.DepthChart.CLI.csproj

