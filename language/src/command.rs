/// A Citizen is anything that can be referenced
#[derive(Debug, Clone)]
pub enum Citizen {
    Identifier(String),
    Str(String),
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
            // if we're comparing two strings their value has to be the same
            (Citizen::Str(s), Citizen::Str(o)) => s == o,
            // anything else and its not the same
            _ => false,
        }
    }
}

pub type ParseError = &'static str;

enum Either<A, B> {
    Left(A),
    Right(B),
}

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
    match words.split_first() {
        // if the first word is "quote"
        Some((head, tail)) if *head == "quote" => {
            // we have to keep consuming until we find another quote
            // we'll start by building up a left value with everything
            // and when we find a right value we'll just return that
            let body =
                tail.into_iter()
                    .fold(Either::Left("".to_string()), |prev, next| match prev {
                        // if we're still looking then look at the next word
                        Either::Left(previous_value) => match next {
                            // if the next word is a quote, we're done just return the previous value
                            &"quote" => Either::Right(previous_value),
                            // we're still looking
                            _ => Either::Left(previous_value + " " + next),
                        },
                        // we're done looking
                        v => v,
                    });

            match body {
                // if we
                Either::Right(value) => Result::Ok(vec![Citizen::Str(value[1..].to_string())]),
                // we didn't find the unquote
                Either::Left(_) => Result::Err("Did not find matching quote"),
            }
        }
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
        let table = vec![
            (
                "hello world",
                Citizen::Identifier("hello world".to_string()),
            ),
            (
                "quote hello world quote",
                Citizen::Str("hello world".to_string()),
            ),
        ];

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
