namespace Sumer.Tests

open NUnit.Framework
open Sumer.Command


[<TestFixture>]
type LangTests () =
    [<Test>]
    member this.ParseEmptyCommand() =
        // parse a test command
        match ParseCommand("") with
        | ParseError err -> Assert.That(true, Is.EqualTo(false), err)
        | ParseResult cmd ->
            // the empty string is a command with no tasks
            Assert.That(cmd.Tasks.Length, Is.EqualTo(0))

    [<Test>]
    member this.ParseSimpleCommand() =
        // parse a test command
        match ParseCommand("hello") with
        | ParseError err -> Assert.That(true, Is.EqualTo(false), err)
        | ParseResult cmd ->
            // make sure there is only 1 task
            Assert.That(cmd.Tasks.Length, Is.EqualTo(1))
            let task = cmd.Tasks.[0]

            // make sure it has the right action and arguments
            Assert.That(task.Action, Is.EqualTo("hello"))
            Assert.That(task.Arguments, Is.EquivalentTo([]))

    [<Test>]
    member this.ParseCommandWithArgs() =
        // parse a test command
        match ParseCommand("hello world") with
        | ParseError err -> Assert.That(true, Is.EqualTo(false), err)
        | ParseResult cmd ->
            // make sure there is only 1 task
            Assert.That(cmd.Tasks.Length, Is.EqualTo(1))
            let task = cmd.Tasks.[0]

            // make sure it has the right action and arguments
            Assert.That(task.Action, Is.EqualTo("hello"))
            Assert.That(task.Arguments, Is.EquivalentTo([Identifier "world"]))

    [<Test>]
    member this.ParseArguments() =
        // the cases to test
        let cases = [
            ("string", "hello quote world quote", [String "world"]);
        ]

        for (title, command, expected) in cases do
            // parse the command
            match ParseCommand command with
            | ParseError err -> Assert.That(true, Is.EqualTo(false), title + ": " + err)
            | ParseResult res ->
                // make sure the arguments match our expectations
                Assert.That(res.Tasks.[0].Arguments, Is.EquivalentTo(expected))

    [<Test>]
    member this.ParseArgumentErrors() =
        // the cases to test
        let cases = [
            ("string", "hello quote world");
        ]

        for (title, command) in cases do
            // parse the command
            match ParseCommand command with
            | ParseResult _ -> Assert.That(true, Is.EqualTo(false), title)
            | ParseError err ->
                // we encountered an error as expected
                ()
