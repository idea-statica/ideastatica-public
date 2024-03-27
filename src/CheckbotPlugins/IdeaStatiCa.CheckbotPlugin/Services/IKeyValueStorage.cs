using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	/// <summary>
	/// A key-value storing for project related data.
	/// Data are stored with the project and are private for the plugin.
	/// </summary>
	public interface IKeyValueStorage
	{
		/// <summary>
		/// Stores data arbitrary data under a given <paramref name="key"/>.
		/// If the key already exists, it is overwritten.
		/// <paramref name="value"/> must contain at least 1 byte.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"><paramref name="key"/> is an empty string.</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> is zero-length.</exception>
		Task Set(string key, ReadOnlyMemory<byte> value);

		/// <summary>
		/// Gets data with the given <paramref name="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">An argument is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="key"/> is an empty string.</exception>
		/// <exception cref="KeyNotFoundException">Key does not exist.</exception>
		Task<ReadOnlyMemory<byte>> Get(string key);

		/// <summary>
		/// Deletes given <paramref name="key"/>.
		/// Returns <see langword="true"/> if the key was sucessfully deleted.
		/// Returns <see langword="false"/> if the key does not exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">An argument is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="key"/> is an empty string.</exception>
		Task<bool> Delete(string key);

		/// <summary>
		/// Checks if given <paramref name="key"/> exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">An argument is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="key"/> is an empty string.</exception>
		Task<bool> Exists(string key);
	}
}