namespace SumerLang.Tests

open NUnit.Framework
open SumerLang.Runtime
open FsUnit


[<TestFixture>]
type RuntimeTests () =
    [<Test>]
    member this.ExecuteParser_newState() =
        // create a runtime to test against
        let runtime = Runtime(
                            state = "",
                            // we want a command that will always run and updates the state to a known value
                            commands = [Command(Runtime.BlackHoleParser, UpdateState "updated!!!")]
                        )

        // execute a command
        match InputStream.FromString "this could be anything" |> runtime.Execute with 
        // if we encountered an error then the test failed
        | Error(_) -> should equal false true
        // if it succeeded 
        | Ok(_) -> 
            // we should have updated the internal state of runtime
            runtime.State |> should equal "updated!!!"
    
    [<Test>]
    member this.ExecuteParser_noCommand() =
        // create a runtime to test against
        let runtime = Runtime(
                            state = "",
                            commands = []
                        )
        // execute a command
        match InputStream.FromString "this could be anything" |> runtime.Execute with 
        // if we encountered an error then the test passed
        | Error(_) -> ()
        // if it succeeded 
        | Ok(_) -> should equal false true
