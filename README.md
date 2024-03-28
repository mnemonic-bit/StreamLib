
# Overview

This library contains a collection of streams. Each stream implementation
aims at providing a single functionality only on top of the common Stream
or other kind of data source (e.g. String).



## StringStream

The StringStream is a wrapper class which provides a stream interface around
any string instance. For each string that is wrapped, you need a new instance
of a stream.

To bridge the gap between the storage format of a string, which is basically
a list of characters, and the output data type of the stream, a byte array
as it is, you have to specify the encoding to use. By default the StringStream
uses Encoding.Unicode, which is a UTF-16 encoding.

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
