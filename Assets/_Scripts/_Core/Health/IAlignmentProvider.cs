using System;
using Core.Utilities;
using UnityEngine;

namespace Core.Health
{
	/// <summary>
	/// An interface for objects which can provide a team/alignment for damage purposes
	/// </summary>
	public interface IAlignmentProvider : ISerializableInterface
	{
		/// <summary>
		/// Gets whether this alignment can harm another
		/// </summary>
		bool CanHarm(IAlignmentProvider other);

        bool IsFriend(IAlignmentProvider other);
    }

    /// <summary>
    /// Concrete serializable version of interface above
    /// </summary>
    [Serializable]
    public class SerializableIAlignmentProvider : SerializableInterface<IAlignmentProvider>
    {
      
    }
}