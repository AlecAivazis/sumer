// externals
use std::collections::HashMap;
// locals
use super::command;

/// A sope is a mapping from identifiers to values
type Scope = HashMap<String, command::Citizen>;

// given some initial state, execute the following command
pub fn execute<T>(
    initial_state: T,
    scope: Scope,
    cmd: command::Command,
) -> Result<T, &'static str> {
    // make sure we have the action defined in the scope
    if !scope.contains_key(&cmd.action) {
        return Err("Action not defined");
    }

    // don't mutate the state just yet
    Ok(initial_state)
}

#[cfg(test)]
mod tests {

    use super::*;

    #[test]
    fn error_cases() {
        // the runtime error cases
        let cases = vec![("Undefined action", "print", HashMap::new())];

        for (title, command_text, scope) in cases {
            // parse the text
            let command = crate::command::parse(command_text.to_string()).unwrap();

            // execute the command
            match execute("", scope, command) {
                Ok(_) => assert!(false, title),
                // there was an error so the test passed
                Err(_) => (),
            }
        }
    }
}
