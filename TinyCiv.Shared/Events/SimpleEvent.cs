namespace TinyCiv.Shared.Events;

// For working demo this is okay,
// later will introduce different event sending/handling when communicating
// with server and client. Most likely will be generic
public record SimpleEvent(string Message);