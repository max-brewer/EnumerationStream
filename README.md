# EnumerationStream
For converting enumerations lazily into streams.

It is intended to help enable lazy evaluated enumerations to be processed without having to load the whole thing into memory.
The use case would be batch processes with large amounts of data on low spec systems.

## ToDo (In no particular order)
* Consider implementing as a StreamReader instead of a stream. This may alow more flexibility.
* Implement for writing.
* Allow other serialisation strategies.
