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
            // parse the input
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

    [<Test>]
    member this.ExecuteScopedFunctions() =
        match ParseCommand("print quote hello quote") with
        | ParseError err -> Assert.That(true, Is.EqualTo(false), err)
        | ParseResult cmd ->
            let mutable called = false
            let print = fun (bindings: Binding list) ->
                called <- true

                []

            // instantiate an executor with print defined as above
            let exec = Executor (Scope [ ("print", Function print)])

            // execute the command
            match exec.Execute "" cmd with
            // there was an error
            | (_, errs) when errs.Length > 0 ->
                // test failed.
                Assert.That(true, Is.EqualTo(false), "Encountered error.")
            // no error
            | _ ->
                // make sure that called is true
                Assert.That(called, Is.EqualTo(true))
