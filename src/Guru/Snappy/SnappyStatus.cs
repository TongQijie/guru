namespace Guru.Snappy
{
    /// <summary>
	/// Status of operation.
	/// </summary>
	public enum SnappyStatus
    {
        /// <summary>OK.</summary>
        OK = 0,

        /// <summary>The input is corrupted.</summary>
        InvalidInput = 1,

        /// <summary>Output buffer is invalid.</summary>
        BufferTooSmall = 2,
    }
}
