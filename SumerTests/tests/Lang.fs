namespace Sumer.Tests

open NUnit.Framework
open Sumer.Language


[<TestFixture>]
type LangTests () =
    [<Test>]
    member this.ParseSimpleCommand() =
        // parse a test command
        let command = ParseCommand("hello world")

        // make sure there is only 1 task
        Assert.That(command.Tasks.Length, Is.EqualTo(1))

        // and that task has the action "hello"
        Assert.That(command.Tasks.[0].Action, Is.EqualTo("hello"))

