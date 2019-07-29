namespace SumerLang.Tests

open NUnit.Framework
open SumerLang.Runtime
open FsUnit


[<TestFixture>]
type RuntimeTests () =
    [<Test>]
    member this.ExecuteParser_newState() =
        // we need to test a command that just sets the state
        let updateParser = fun (input: InputStream) -> Some(Ok(Map.empty))
        // we do want an operation that sets the new state
        let updateOperation = NewState "updated!!!"

        // create a runtime to test against
        let runtime = Runtime(
                            state = "",
                            commands = [Command(updateParser, updateOperation)]
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
