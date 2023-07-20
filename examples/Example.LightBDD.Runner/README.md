## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.XUnit2** integration.

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

## Running tests

It is possible to run xunit based LightBDD tests in various ways, which are described below.

The project directory contains `run-*.cmd` batch scripts allowing to see test execution in action.

After running the tests, please check the `bin\Debug\netXXX\Reports` directory to see the execution reports.

### Using Visual Studio Test Explorer
To run tests from Test Explorer the **xunit.runner.visualstudio** package has to be added.

### Using dotnet test
To run tests with `dotnet test` command the **xunit.runner.visualstudio** package has to be added.

As this command does not display output of successful tests, it is recommended to call this command with `--logger:"console;verbosity=detailed"` argument to change this behavior.

Please note that as of today, running `dotnet test` for xunit2 does not display any progress of currently executing tests so with this command it is not possible to track the execution of long running tests.

Example usage: `> run-dotnet-test.cmd`

### Using xunit.console.exe
To run tests with `xunit.console.exe` the [xunit.runner.console](https://www.nuget.org/packages/xunit.runner.console) has to be fetched during build process and executed against the compiled dll.

Using this method will also allow to track the progress of executing tests.

Example usage: `> run-xunit.console.cmd`