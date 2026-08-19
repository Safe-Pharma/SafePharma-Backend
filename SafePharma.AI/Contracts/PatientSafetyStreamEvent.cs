namespace SafePharma.AI.Contracts
{
    /// <summary>
    /// One event in the streaming version of a safety check. Multiple patients'
    /// events are interleaved on the same stream, each tagged with PatientRef
    /// so the UI can route updates to the right part of the screen.
    /// </summary>
    public record PatientSafetyStreamEvent
    {
        public required string PatientRef { get; init; }
        public required PatientSafetyStreamEventType Type { get; init; }

        /// <summary>Short human-readable status, e.g. "Checking drug interactions...". Set when Type = Progress.</summary>
        public string? Message { get; init; }

        /// <summary>Set only when Type = Result.</summary>
        public PatientSafetyResult? Result { get; init; }
    }

    public enum PatientSafetyStreamEventType
    {
        Progress,
        Result
    }
}