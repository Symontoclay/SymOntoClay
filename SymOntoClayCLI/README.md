# SymOntoClay CLI

Running CLI without arguments prints help.

<!-- CLI-HELP-START -->
## CLI arguments

* `h` - prints help.
* `help` - prints help.
* `run <NPC file name>` - runs NPC and waits when you write `exit` for exit.
* `run` - runs current NPC and waits when you write `exit` for exit. The current NPC is detected by locators.
* `exit` - ends running NPC.
* `new [-npc] <NPC name>` - creates new NPC in new or existing worldspace. For creating NPC in existing worldspace runs command 'new' in worldspace directory.
* `n` - alias of 'new' command.
* `new -thing <NPC name>` - creates new Thing in new or existing worldspace. For creating NPC in existing worldspace runs command 'new' in worldspace directory.
* `new -world <NPC name>` - creates new worldspace.
* `new -w <NPC name>` - creates new worldspace.
* `new -lib <Lib name>` - creates new library.
* `new -l <Lib name>` - creates new library.
* `new -nav <Nav object name>` - creates new navigation object.
* `new -player <Player name>` - creates new player.
* `new -p <Player name>` - creates new player.
* `install <Lib name>` - installs library.
* `version` - prints current version of SymOntoClay.
* `v` - alias of 'version' command.

## Additional CLI arguments

* `-nologo` - suppresses printing header.
* `-timeout <milliseconds>` - terminates CLI after `<milliseconds>`. CLI will return 0.
* `-nlp` - enables NLP (natural language processing).
<!-- CLI-HELP-END -->