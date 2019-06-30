namespace Sumer.Tests

open NUnit.Framework
open Sumer.Command


[<TestFixture>]
type LangTests () =
    [<Test>]
    member this.ParseEmptyCommand() =
        // parse a test command
        match ParseCommand("") with
        | ParseError error ->
            Assert.That(error, Is.Not.EqualTo(""))
        | ParseResult _ ->
            Assert.That(true, Is.EqualTo(false), "did not encounter error")

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
            Assert.That(task.Arguments, Is.EquivalentTo(["world"]))
