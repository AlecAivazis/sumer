
/// A Citizen is anything that can be referenced
#[derive(Debug, Clone)]
enum Citizen {
    Identifier(str),
    Str(str),
    Function(fn),
}



#[cfg(test)]
mod tests {
    // import the parent scope
    use super::*;

    #[test]
    fn parse_command() {

    }

}