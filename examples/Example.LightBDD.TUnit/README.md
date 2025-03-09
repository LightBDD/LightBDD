## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.TUnit** integration.

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

## Running tests

It is possible to run nunit based LightBDD tests in various ways, which are described below.

The project directory contains `run-*.cmd` batch scripts allowing to see test execution in action.

After running the tests, please check the `bin\Debug\netXXX\Reports` directory to see the execution reports.

### Using Visual Studio Test Explorer
TUnit natively supports running tests from Test Explorer.

### Using dotnet test
TUnit natively supports `dotnet test` command.

When executed, the progress of currently executing tests will be printed on console allowing to track the execution of long running tests.

Example usage: `> run-dotnet-test.cmd`