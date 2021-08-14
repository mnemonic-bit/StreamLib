# ThrottledStream

This small library contains an implementation of a throttled stream.
The stream operates on a base stream and throttles down the rate of
bytes-per-second to a fixed maximum values for either Read() or Write()
method calls, or both if you choose to.

## Quick Start

To get startet with a ThrottledStream, just wrap your current stream
instance with an instance of a ThrottledStream, and use that reference
instead:

```
using var baseStream = new MemoryStream(buffer);
using var throttledStream = new ThrottledStream(baseStream, 4096);
// from here use throttledStream instead of baseStream...
```

The constructor needs a base stream as well as the speed-limit the
ThrottledStream must enforce, in the example above we've chosen 4096
bytes per second, i.e. 4 kb.

By default the ThrottledStream only throttles the speed of reading
from the base-stream. We can change that, by setting one of the optional
parameters to the constructor, as is shown in the next example:

```
using var baseStream = new MemoryStream(buffer);
using var throttledStream = new ThrottledStream(baseStream, 4096, true, true);
// from here use throttledStream instead of baseStream...
```

The example above created a ThrottledStream which throttles read and
write operations.