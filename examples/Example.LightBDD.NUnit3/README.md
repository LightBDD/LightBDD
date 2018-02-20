## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.NUnit3** integration.

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

## Integration notes
As NUnit3 supports concurrent test execution it is possible to enable it for LightBDD scenarios as well.

Please note that as nunit shares the same test class instance between tests, enabling parallel execution of the tests belonging to the same class may lead to concurrency issues if scenario steps shares data with fields declared in feature class.
In such scenario please consider using contextual scenarios as they would allow to share state between steps in thread safe way. More information about contextual scenarios can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#contextual-scenarios

## Running tests

It is possible to run nunit based LightBDD tests in various ways, which are described below.

The project directory contains `run-*.cmd` batch scripts allowing to see test execution in action.

After running the tests, please check the `bin\Debug\netXXX\Reports` directory to see the execution reports.

### Using Visual Studio Test Explorer
To run tests from Test Explorer the **NUnit3TestAdapter** package has to be added.

### Using dotnet test
To run tests with `dotnet test` command the **NUnit3TestAdapter** package has to be added.

When executed, the progress of currently executing tests will be printed on console allowing to track the execution of long running tests.

Example usage: `> run-dotnet-test.cmd`

### Using nunit3-console.exe
To run tests with `nunit3-console.exe` the [NUnit.ConsoleRunner](https://www.nuget.org/packages/NUnit.ConsoleRunner) has to be fetched during build process and executed against the compiled dll.

Similarly to above command, `nunit3-console.exe` will print current progress of executing tests but as a contrary to `dotnet test` it will also print an execution summary of each test when it is finished.

Example usage: `> run-nunit3-console.cmd`