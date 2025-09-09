# TUSP

**TUSP (The Ultimate Streaming Protocol)** is an experimental protocol for low-latency audio and video streaming. It is built on top of UDP and is designed to work with fragmented MP4 (fMP4) segments.

---

## Features

- Streaming of fMP4 video segments (init + moof+mdat) over UDP
- Chunking of segments for safe transmission over networks with limited MTU
- Packet numbering (`SequenceNumber`) and session identifiers (`SessionId`)
- Support for arbitrary headers (`Headers`) for metadata
- Optional acknowledgment (`Ack`) for retransmission of lost packets
- Frame timestamps (PTS/DTS) can be used from fMP4 or passed explicitly in headers

---

## TUSP Package Structure

```text
TuspPackage
├─ SessionId (uint32)                   // Session identifier
├─ MessageType (int32)                  // Message type (Ping, Init, Data, etc.)
├─ SequenceNumber (uint32)              // Packet sequence number
├─ Headers (Dictionary<string,string>)  // Additional metadata
├─ PayloadLength (uint32)               // Length of Data segment 
└─ Payload (byte[])                     // Data (fMP4 chunk or text)
```

## Message Types

Init    -   Initial message for establishing connection (Heandshake request)

Ack     -   Heandshake response. May contain Secure key if establishing connection is secure

Ping    -   Healthcheck request

Pong    -   Healthcheck response

Data    -   Message containing data chunk

End     -   Message determining closing connection

Error   -   Error message 

## Video Chunking

Message is divided into 1200 byte chunks for safe transmition

Each package contains SequenceNumber for determening chunk index

Data packages contain header "IsLast"

## Usage

You may start with ping to determine if connection is available

Then send Init message

If acknowledgment has succeded send data reqeust and start receiving data chunks