using UnityEngine;
using System.Collections;

namespace AIBehavior
{

    public enum SearchType
    {
        Enemy,
        Friend,
        All
    }



    public enum AnimationType
	{
		Auto,
		Mecanim,
		Legacy,
		TwoD
	};


	public enum CachePoint
	{
        Init,
		StateChanged,
		EveryFrame
	}


	public enum DistanceNegotiation
	{
		Default,
		Any,
		All
	}


	public enum ResetMode
	{
		OnStateEnter,
		WhenTriggered,
		Never
	}
}