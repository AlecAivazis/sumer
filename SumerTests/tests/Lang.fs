namespace Sumer.Tests

open NUnit.Framework
open Sumer.Language


[<TestFixture>]
type LangTests () =
    [<Test>]
    member this.ParseSimpleCommand() =
        // parse a test command
        let command = ParseCommand("hello")

        // make sure there is only 1 task
        Assert.That(command.Tasks.Length, Is.EqualTo(1))
        let task = command.Tasks.[0]

        // make sure it has the right actions and arguments
        Assert.That(task.Action, Is.EqualTo("hello"))
        Assert.That(task.Arguments, Is.EquivalentTo([]))

    [<Test>]
    member this.ParseCommandWithArgs() =
        // parse a test command
        let command = ParseCommand("hello world")

        // make sure there is only 1 task
        Assert.That(command.Tasks.Length, Is.EqualTo(1))
        let task = command.Tasks.[0]

        // make sure it has the right actions and arguments
        Assert.That(task.Action, Is.EqualTo("hello"))
        Assert.That(task.Arguments, Is.EquivalentTo(["world"]))
