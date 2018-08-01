using System;

namespace CardinalInventoryApp.iOS.ScanBarcode
{
    /// <summary>
	/// Strongly typed EventArgs class
	/// </summary>
	public class EventArgsT<T> : EventArgs
    {
        public T Value { get; }
        public EventArgsT(T val) => Value = val;
    }
}