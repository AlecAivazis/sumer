namespace Sumer.Tests

open NUnit.Framework
open Sumer.Command
open Sumer.Executor


[<TestFixture>]
type ExecutorTests () =

    [<Test>]
    member this.ErrorCases() =
        let cases: (string * string * (string * Citizen) list) list = [
            ("unknown identifier as action", "print", []);
        ]

        // try each case
        for (message, case, scope) in cases do
            // parse the inputx
            match ParseCommand(case) with
            | ParseError err -> Assert.That(true, Is.EqualTo(false), err)
            | ParseResult cmd ->
                // instantiate an executor
                let exec = Executor (Scope scope)

                // execute the command
                match exec.Execute "" cmd with
                // if there are no errors
                | (_, []) ->
                    // no error was found. test failed.
                    Assert.That(true, Is.EqualTo(false), "Did not encounter an error: " + message)

                // there are errors
                | (_, errs) ->
                    // an error was found so the test passed
                    ()
