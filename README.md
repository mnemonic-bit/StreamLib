
# Overview

This library contains a collection of streams. Each stream implementation
aims at providing a single functionality only on top of a common Stream
implementation or other kind of data source (e.g. String).

The Streams this library provides are:

* EphemeralStream: provides a stream which holds all its contents in memory
* EventStream: provides a stream which posts events on each call to the stream
* MeteringStream: wraps a stream around any other stream implementation and provides speed measurements of the base stream's read and write operations
* StringStream: wraps a stream interface around a String instance without additional memory allocation
* ThrottledStream: wraps a stream around any other Stream implementation and helps throttling the read and write speeds of that base stream


## EphemeralStream

The EphemeralStream implements a Stream and holds all its contents in memory.
For this it utilizes the ArrayPool for borrowing and returning its chunks of
memory. The size of each chunk can be chosen, but by default it is much smaller
than 80k of bytes, preventing the chunks from being moved to the large object
heap of the runtime, which should result in performance improvements. Also the
chunks of memory are allocated only on demand, i.e. on its first access to that
region of the stream.

### Quick Start

There really is not much to say here, its just create a new instance, and
use it.

```
var ephemeralStream = new EphemeralStream();
```

If you want more control on some parameters that govern the behaviour of the
EphemeralStream, you can set them when the instance is created. Things that can
be set are

* numberOfChunks: the initial number of chunks the stream is prepared to hold. This is not a fixed number, if the capacity increases above the current limit of this stream, the number of chunks will also be adapted to accomodate the new requirements
* chunkSize: the size of each chunk of memory which will be allocated just in time when it is needed the first time
* fixedSize: if set to true, the stream cannot change its size, i.e. that its size is determined by numberOfChunks * chunkSize


## EventStream

The EventStream can be used for debugging purposes. It wraps around a base-stream
and adds events to each call that the user of the EventStream can register to. This
makes it easy to add logging facilities to specific streams in an application.


## MeteringStream

The MeteringStream is a wrapper class which provides metering capabilities to
the base stream. Users of this stream can register for an event which is sent
out approximately once per second, which contains information about the progress
of the operation.

### Quick Start

To use the MeteringStream we only need to wrap the original stream. In addition
to that, we can register delegates to receive read or write progress events which
contain information about the total time elapsed, the total number of bytes,
and the current speed.

```
Stream baseStream = ...
var meteringStream = new MeteringStream(baseStream);
// For read progress add your handler by calling:
meteringStream.AddReadEventListener(mea => { ... });
// For write progress add your handler by calling:
meteringStream.AddWriteEventListener(mea => { ... });
// Or in case you want one delegate to handle both events:
meteringStream.AddEventListener(mea => { ... });
```


## StringStream

The StringStream is a wrapper class which provides a stream interface around
any string instance. For each string that is wrapped, you need a new instance
of a stream.

To bridge the gap between the storage format of a string, which is basically
a list of characters, and the output data type of the stream, a byte array
as it is, you have to specify the encoding to use. By default the StringStream
uses Encoding.Unicode, which is a UTF-16 encoding, but you can choose any encoding
that fits your needs.

### Quick Start

A StringStream can be created with the default encoding (UTF-16) with the
constuctor that accepts the string as the source of text to wrap a stream
around.

```
string text = "SOME-TEST-TEXT";
using StringStream stream = new StringStream(text);
```


## ThrottledStream

The stream operates on a base stream and throttles down the rate of
bytes-per-second to a fixed maximum values for either Read() or Write()
method calls, or both if you choose to.

### Quick Start

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
bytes per second, i.e. 4 kb/s.

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


## TextPositionReader

This implementation of a TextReader can be used to get the position in the
text you are currently reading from.

### Quick Start

To retreive the position of the text you read from a TextReader or one of
its subclasses, e.g. StreamReader or StringReader, just wrap you instance
around an instance of TextPositinReader:

```
using var baseReader = new TextReader(str);
using var textPositinReader = new TextPositionReader(baseReader);
// from here use the textPositinReader instead of baseReader...
```

## Stream Extension Methods

This library contains a few Stream extension methods which implement recurring
patters when it comes to Stream interaction.

We give a brief overview of the extension methods here. More details are
due to a forthcoming documentation of this library. And as always, the source-code
is the most accurate self-documentation.

* DecodeBase64: uses the contents of the stream to create a new stream which will decode its content from Base64.
* EncodeBAse64: uses the contents of the stream to create a new stream which will contain a Base64 encode form of that content.
* ReadString: reads the whole contents of the stream and creates a string from this contents. The default encoding is UTF8, but can be optionally set to any desired encoding.
* ReadStringAsync: the same as ReadString, but uses the asynchronous API of the stream instead.
* ReadAll: simply reads the whole contents of the stream and returns a byte[] of that contents.
* ReadAllAsync: the same as ReadAll, but uses the asynchronous API of the stream instead.
* WriteAllTo: copies the whole contents of the stream to a destinatoin stream.


## XmlReader Extension Methods

There are also extension methods for the XmlReader for some common usage patterns.

* ToStream: returns a Stream to read the contents of an XmlReader
* ToStreamAsync: returns a Stream to access the contents of the XmlReader asynchronously
