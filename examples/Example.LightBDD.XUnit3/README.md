## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.XUnit3** integration (xUnit v3).

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

## Running tests

It is possible to run xUnit v3 based LightBDD tests in various ways, which are described below.

The project directory contains `run-dotnet-test.cmd` batch script allowing to see test execution in action.

After running the tests, please check the `bin\Debug\net8.0\Reports` directory to see the execution reports.

### Using Visual Studio Test Explorer
To run tests from Test Explorer the **xunit.runner.visualstudio** package has to be added.

### Using dotnet test
To run tests with `dotnet test` command the **xunit.runner.visualstudio** package has to be added.

As this command does not display output of successful tests, it is recommended to call this command with `--logger:"console;verbosity=detailed"` argument to change this behavior.

Example usage: `> run-dotnet-test.cmd`

### Running as executable
xUnit v3 test assemblies are standalone executables. You can run them directly:

```
> bin\Debug\net8.0\Example.LightBDD.XUnit3.exe
```
