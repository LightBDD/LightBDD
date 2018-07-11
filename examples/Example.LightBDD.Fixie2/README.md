## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.Fixie2** integration.

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

## Running tests

It is possible to run Fixie based LightBDD tests in various ways, which are described below.

The project directory contains `run-*.cmd` batch scripts allowing to see test execution in action.

After running the tests, please check the `bin\Debug\netXXX\Reports` directory to see the execution reports.

### Using Visual Studio Test Explorer
To run tests from Test Explorer the `<DotNetCliToolReference Include="Fixie.Console" Version="..." />` package dependency has to be added to the project.

### Using dotnet test
To run tests with `dotnet test` command the  `<PackageReference Include="Fixie" Version="..." />` package dependency has to be added to the project.

Please note that as of today, running `dotnet test` for Fixie does not display any progress of currently executing tests, so with this command it is not possible to track the execution of long running tests.

Example usage: `> run-dotnet-test.cmd`

### Using dotnet fixie
To run tests with `dotnet fixie` command the `<DotNetCliToolReference Include="Fixie.Console" Version="..." />` package dependency has to be added to the project.

Similarily to `dotnet test` command, this one will *not* display any progress of executing tests, not allowing to track long running tests, however unlike previous command it will display the details of all tests when they are finished.

Example usage: `> run-dotnet-fixie.cmd`

_Note: `dotnet fixie` command works only if it is executed from project directory_

### Filtering scenarios by categories
To run only a subset of tests it is possible to run dotnet fixie command with category filters as follows: `dotnet fixie -- --category MyCategory1 --category MyCategory2`.
When executed, only scenarios with `[ScenarioCategory("MyCategory1")]` or `[ScenarioCategory("MyCategory2")]` attributes being present on scenario method or fixture class will run.

Please note, that category filters will be passed only if test project contains LightBdd discovery conventions defined as follows:
```c#
internal class WithLightBddConventions : LightBddDiscoveryConvention
{
    public WithLightBddConventions(string[] category)
    {
        IncludeCategories(category);
    }
}
```

Example usage: `> run-dotnet-fixie-filter-categories.cmd`