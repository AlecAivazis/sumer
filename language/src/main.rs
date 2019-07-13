// external crates
use std::collections::HashMap;

// load the internal modules
mod command;
mod executor;

#[allow(unused_must_use)]
fn main() {
    match command::parse("hello".to_string()) {
        Err(err) => println!("{}", err),
        Ok(cmd) => {
            executor::execute("", HashMap::new(), cmd);
            ()
        }
    }
}
