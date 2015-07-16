Edison
======

Edison is designed to be simpler and more performant unit/integration testing framework for .NET projects.
Many features, such as Attributes, are similar to NUnit for a more fluid transition.


License
=======

Edison is completely open sourced and free under the MIT License.


Usage
=====

Framework
---------

Using Edison is very similar to NUnit. Yout have a [Test] Attribute with varying other Attributes to create your tests. An example would be:

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
	[Case(1)]
	[Case(2)]
	public void Test(int value)
	{
		AssertFactory.Instance.AreEqual(2, value, "Argh no it's an error!!!1");
	}
}
```

Here you can see that this is mostly very similar to NUnit. Edison has been designed this way to make it easier for people to transition over.

In the example above we have:
* A `TestFixture` which contains multiple `Test`s to be run
* A `Setup` method which is run before each `Test`
* A `Teardown` method which is run after each `Test`. This can optionally take a TestResult object.
* And one `Test` method, which has a `Category` of "Name", and two possible `Case`s to run the Test with as 1 and 2.

For people who know NUnit, you may have noticed that 'TestCase' is just 'Case' in Edison.

Furthermore, there's the Asserts class. In Edison the main `Assert` class implements the `IAssert` interface. To use the `Assert` class you can either create an instance of it for each Test, or you can use the `AssertFactory` class.
The `AssertFactory` class contains a lazy Instance property which returns the `IAssert` class being used for the test assembly. This means you can create your own `CustomAssert` class that inherits `IAssert` and do `AssertFactory.Instance = new CustomAssert()` and any calls to `AssertFactory.Instance` will return your `CustomAssert`. This makes it far simpler to have your own assert logic in your test framework.


Console and Engine
------------------

Edison has the inbuilt functionality to run tests in parallel threads. By default tests are run in a single thread however, by suppling the `-t <value>` parameter from the command-line the tests will be run in that many threads. If you supply a number of threads that exceeds the number of TestFixtures, then the number of threads became the number of TestFixtures.

Edison has the following flow when running tests per assembly:

```
SetupFixture -> Setup
 |
TestFixture -> TestFixtureSetup
 |
TestFixture -> Setup
 |
TestFixture -> Test
 |
TestFixture -> Teardown
 |
TestFixture -> TestFixtureTeardown
 |
SetupFixture -> Teardown
```

Example of running a test assembly from the command-line:

```bash
Edison.Console.exe -a path/to/test/assembly.dll -t 2 -ot json
```

This will run the tests across two threads from the assembly.dll file. The results of the tests will be output to the working directory in json format.
To see more parameters use:

```bash
Edison.Console.exe -help
```


Features
========

* Framework with Attributes and Assert class for writing unit/integration tests.
* Console application from which to run your tests - with useful inputs like TestResultURL to send results.


To Do
=====

* GUI
* Website with service for automatically running tests
* More can be found on Trello which I'll link at some point
