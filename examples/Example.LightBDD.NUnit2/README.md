## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.NUnit2** integration.

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

## Integration notes
Please note that LightBDD.NUnit2 does not support capturing parameterized scenarios. While it is possible to use parameterized scenario methods, the method parameters will not be included in scenario name.

## Running tests

It is possible to run nunit based LightBDD tests in various ways, which are described below.

The project directory contains `run-*.cmd` batch scripts allowing to see test execution in action.

After running the tests, please check the `bin\Debug\netXXX\Reports` directory to see the execution reports.

### Using Visual Studio Test Explorer
To run tests from Test Explorer the **NUnitTestAdapter** package has to be added.

### Using nunit-console.exe
To run tests with `nunit-console.exe` the [NUnit.Runners](https://www.nuget.org/packages/NUnit.Runners) has to be fetched during build process and executed against the compiled dll.

When executed, the progress of currently executing tests will be printed on console allowing to track the execution of long running tests.

Example usage: `> run-nunit-console.cmd`