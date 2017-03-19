# EnumerationStream
For converting enumerations lazily into streams.

It is intended to help enable lazy evaluated enumerations to be processed without having to load the whole thing into memory.
The use case would be batch processes with large amounts of data on low spec systems.

## ToDo (In no particular order)
* Consider implementing as a StreamReader instead of a stream. This may alow more flexibility.
* Implement for writing.
* Allow other serialisation strategies.

## Update
It looks like there was the JsonWriter class from Newtonsoft Json all along and I just didn't know about it.
The whole idea seems a little pointless when it requires so little code, but there you go. A learning experience for me.
That's the lovely thing about Newtonsoft Json. Every time you thought it did everything you could possibly ask for, you find another thing that it can do that you never even considered asking it to. #bestlibraryever.
