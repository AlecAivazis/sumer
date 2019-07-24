task "build" {
    description = "Compile the sumer dll and copy it into the unity project"
    pipeline = [
        // compile the library
        "dotnet publish SumerLang",
    ]
}

task "tests" {
    description = "Run the tests associated with this project"
    pipeline = [
        "dotnet test SumerLang.Tests"
    ]
}
