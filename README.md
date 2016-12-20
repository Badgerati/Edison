# Edison

[![Build Status](https://travis-ci.org/Badgerati/Edison.svg?branch=master)](https://travis-ci.org/Badgerati/Edison)
[![Build status](https://ci.appveyor.com/api/projects/status/i4fa3crkr6mrnjgt?svg=true)](https://ci.appveyor.com/project/Badgerati/edison)
[![Code Climate](https://codeclimate.com/github/Badgerati/Edison/badges/gpa.svg)](https://codeclimate.com/github/Badgerati/Edison)
[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/Badgerati/Edison/master/LICENSE.txt)

Edison is designed to be a more performant unit/integration testing framework for .NET projects.
Many features, such as Attributes, are similar to other test frameworks for a more fluid transition.

# Features

* Framework with Attributes and Assert class for writing unit/integration tests.
* Can run tests in parallel.
* Output can be in XML, JSON, CSV, HTML, Markdown or just plain text.
* Ability to send test results, in real-time, to a URL for later analysis.
* Console application from which to run your tests.
* GUI for a more visual look on running tests.
* Ability to re-run tests that have failed, for ones that may pass if run seconds later.
* Run test cases and repeats in parallel.
* Supply a solution file for extracting test assemblies.
* Ability to store versioned console parameters in an Edisonfile.
* Basic support for browser testing
* Support for TestDriven.NET

# Installing Edison

You can now install Edison using [Chocolatey](https://chocolatey.org/packages/edison "Chocolatey") using the following:

```bash
choco install edison
```

Also, you can now get the [framework](https://www.nuget.org/packages/Edison.Framework "framework") and [console](https://www.nuget.org/packages/Edison.Console "console") on NuGet as well:

```bash
Install-Package Edison.Framework
Install-Package Edison.TestDriven # < this is the core framework, but has support for TestDriven.NET
Install-Package Edison.Console
```

# Usage
## Framework

Using Edison is very similar to other test frameworks. You have a `[Test]` Attribute with varying other Attributes to create your tests. An example would be:

```C#
[TestFixture]
public class TestClass
{
    [Setup]
    public void Setup()
    {
        //stuff
    }

    [Teardown]
    public void Teardown(TestResult result)
    {
        //stuff
    }

    [Test]
    [Category("Name")]
    [TestCase(1)]
    [TestCase(2)]
    public void Test(int value)
    {
        AssertFactory.Instance.AreEqual(2, value, "Argh no it's an error!!!1");
    }
}
```

Here you can see that this is similar to other test frameworks. Edison has been designed this way to make it easier for people to transition over.

In the example above we have:
* A `TestFixture` which contains multiple `Test`s to be run
* A `Setup` method which is run before each `Test`
* A `Teardown` method which is run after each `Test`. This can optionally take a TestResult object.
* And one `Test` method, which has a `Category` of "Name", and two possible `TestCase`s to run the Test with as 1 and 2.

Furthermore, there's the Asserts class. In Edison the main `Assert` class implements the `IAssert` interface. To use the `Assert` class you can either create an instance of it for each Test, or you can use the `AssertFactory` class.
The `AssertFactory` class contains a lazy Instance property which returns the `IAssert` class being used for the test assembly. This means you can create your own `CustomAssert` class that inherits `IAssert` and do `AssertFactory.Instance = new CustomAssert()` and any calls to `AssertFactory.Instance` will return your `CustomAssert`. This makes it far simpler to have your own assert logic in your test framework. If you don't set the `AssertFactory.Instance` then this is default to be the inbuilt `Assert` logic.

## Console and Engine

Edison has the inbuilt functionality to run tests in parallel threads. By default tests are run in a single thread however, by suppling the `--t <value>` parameter from the command-line the tests will be run in that many threads. If you supply a number of threads that exceeds the number of TestFixtures, then the number of threads will become the number of TestFixtures.

Edison has the following flow when running tests per assembly:

```
SetupFixture -> Setup
 |
TestFixture -> TestFixtureSetup
 |
TestFixture -> Setup
 |
TestFixture -> Tests and TestCases
 |
TestFixture -> Teardown
 |
TestFixture -> TestFixtureTeardown
 |
SetupFixture -> Teardown
```

Example of running a test assembly from the command-line:

```bash
.\Edison.Console.exe --a path/to/test/assembly.dll --ft 2 --ot json
```

This will run the test fixtures across two threads (--ft) from the assembly.dll file (--a). The results of the tests will be output to the working directory in json format (--ot).

Do you have your own in-house test history storage? You can post the test results from Edison.Console to a given URL. Also you can specify a Test Run ID to help uniquely identify the run the results came from:

```bash
.\Edison.Console.exe --a path/to/test/assembly.dll --ft 2 --dfo --dco --ot json --url http://someurl.com --tid 702
```

Again this will run the test fixtures across two threads however, this time we won't be creating an output file (--dfo) or outputting the results to the console (--dco). Instead, the results will be posted to the passed URL (--url) and also use the test run ID specified (--tid).

To see more parameters use:

```bash
.\Edison.Console.exe --help
```

## Edisonfile

The `Edisonfile` allows you to save, and version control, the arguments that you can supply to the console application. The file is of YAML format and should be saved at the root of your repository.
The following is an example of the format:

```yaml
assemblies:
  - "./path/to/test.dll"
  - "./path/to/other/test.dll"

disable_test_output: true
console_output_type: dot
fixture_threads: 2
```

To run the application, just run `Edison.Console.exe` at the root, with no arguments supplied. The application will locate the Edisonfile and populate the parameters accordingly.

For example, the above will be just like running:

```bash
Edison.Console.exe --a ./path/to/test.dll, ./path/to/other/test.dll --dto --cot dot --ft 2
```

# Bugs and Feature Requests

For any bugs you may find or features you wish to request, please create an [issue](https://github.com/Badgerati/Edison/issues "Issues") in GitHub.

# License

Edison is completely open sourced and free under the MIT License.
