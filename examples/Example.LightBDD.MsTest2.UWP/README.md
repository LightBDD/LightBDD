## About project
This is a sample project presenting how to create and run LightBDD scenarios with **LightBDD.MsTest2** integration in UWP environment.

The scenarios created within this project are written in different styles to show various features of LightBDD.
Also, for better clarity how LightBDD behaves, some of the scenarios are failing or finishing with ignored status.

**Note:** The main purpose of this sample it is to show that it is possible to run LightBDD tests in UWP, however as I do not have much experience with UWP, there may be easier or better ways to do it.

## Integration notes
To run LightBDD.MsTest2 scenarios in UWP project, please ensure that `System.Console` package is added.

## Running tests in Visual Studio
To run tests from Test Explorer the **MSTest.TestAdapter** package has to be added.

Steps to run tests:
1. Right click on project and click "Set as StartUp Project"
2. Select in menu to run on "Local Machine"
3. Right click on project and click "Deploy"
4. Open Test Explorer and run tests.

After tests are executed, the LightBDD HTML report should open.

Please note that this project is excluded from Release build - it only builds in debug mode.