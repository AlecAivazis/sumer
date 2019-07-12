/// A Citizen is anything that can be referenced
#[derive(Debug, Clone)]
pub enum Citizen {
    Identifier(String),
}

pub struct Command {
    pub action: String,
    pub arguments: Vec<Citizen>,
}

impl PartialEq for Citizen {
    fn eq(&self, other: &Self) -> bool {
        match (self, other) {
            // if we're comparing two idenfitiers their value has to be the same
            (Citizen::Identifier(s), Citizen::Identifier(o)) => s == o,
            // anything else and its not the same
            _ => false,
        }
    }
}

pub type ParseError = &'static str;

/// parse a string into the corresponding Command
pub fn parse(cmd: String) -> Result<Command, ParseError> {
    // if we weren't given a string
    if cmd.len() == 0 {
        // yell loudly
        return Err("Encountered empty command");
    }

    // we have a string to parse, split it on space
    let words: Vec<_> = cmd.split(' ').collect();

    // grab the first word in the list
    let action = words[0].to_string();

    // try to grab the arguments following the action
    match parse_arguments(&words[1..]) {
        Err(err) => Err(err),
        Ok(arg_list) => Ok(Command {
            action: action,
            arguments: arg_list,
        }),
    }
}

fn parse_arguments(words: &[&str]) -> Result<Vec<Citizen>, ParseError> {
    // if there are no words
    if words.len() == 0 {
        // there are no arguments
        return Ok(vec![]);
    }

    // the words can be a number of different things
    match words {
        // otherwise treat it as an identifier
        _ => Result::Ok(vec![Citizen::Identifier(words.join(" "))]),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parse_empty_command() {
        match super::parse("".to_string()) {
            // we got a command (no error)
            Ok(_) => assert!(false, "did not encounter an error"),
            // if we did encounter an error, the tests passed!
            Err(_) => (),
        };
    }

    #[test]
    fn parse_command_no_args() {
        match super::parse("hello".to_string()) {
            // if we encountered an error the test failed
            Err(err) => assert!(false, err),
            Ok(cmd) => {
                // make sure we got a command with the right action
                assert_eq!(cmd.action, "hello");
                // and there are no arguments
                assert_eq!(cmd.arguments.len(), 0);
            }
        };
    }

    #[test]
    fn parse_single_argument() {
        let table = vec![("world", Citizen::Identifier("world".to_string()))];

        for row in table {
            match super::parse("hello ".to_string() + row.0) {
                // if we encountered an error the test failed
                Err(err) => assert!(false, err),
                Ok(cmd) => {
                    // make sure we got a command with the right action
                    assert_eq!(cmd.action, "hello");
                    // and there is only one argument
                    assert_eq!(cmd.arguments.len(), 1);
                    // and it is what we expected
                    assert_eq!(cmd.arguments[0], row.1);
                }
            };
        }
    }
}
