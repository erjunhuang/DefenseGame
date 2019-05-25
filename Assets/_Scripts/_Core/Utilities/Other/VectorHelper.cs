using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
	public static class VectorHelper
	{
		/// <summary>
		/// A helper function that finds the average position of several component objects, 
		/// specifically because they have transforms
		/// </summary>
		/// <param name="components">
		/// The list of components to average
		/// </param>
		/// <typeparam name="TComponent">
		/// The Unity Component which has a transform
		/// </typeparam>
		/// <returns>
		/// The average position
		/// </returns>
		public static Vector3 FindAveragePosition<TComponent>(TComponent[] components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				output += component.transform.position;
			}
			return output / components.Length;
		}

		/// <summary>
		/// A helper function that finds the average position of several component objects, 
		/// specifically because they have transforms
		/// </summary>
		/// <param name="components">
		/// The list of components to average
		/// </param>
		/// <typeparam name="TComponent">
		/// The Unity Component which has a transform
		/// </typeparam>
		/// <returns>
		/// The average velocity
		/// </returns>
		public static Vector3 FindAverageVelocity<TComponent>(TComponent[] components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				var rigidbody = component.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					continue;
				}
				output += rigidbody.velocity;
			}
			return output / components.Length;
		}

		/// <summary>
		/// A helper function that finds the average position of several component objects, 
		/// specifically because they have transforms
		/// </summary>
		/// <param name="components">
		/// The list of components to average
		/// </param>
		/// <typeparam name="TComponent">
		/// The Unity Component which has a transform
		/// </typeparam>
		/// <returns>
		/// The average position
		/// </returns>
		public static Vector3 FindAveragePosition<TComponent>(List<TComponent> components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				output += component.transform.position;
			}
			return output / components.Count;
		}

		/// <summary>
		/// A helper function that finds the average position of several component objects, 
		/// specifically because they have transforms
		/// </summary>
		/// <param name="components">
		/// The list of components to average
		/// </param>
		/// <typeparam name="TComponent">
		/// The Unity Component which has a transform
		/// </typeparam>
		/// <returns>
		/// The average velocity
		/// </returns>
		public static Vector3 FindAverageVelocity<TComponent>(List<TComponent> components) where TComponent : Component
		{
			Vector3 output = Vector3.zero;
			foreach (TComponent component in components)
			{
				if (component == null)
				{
					continue;
				}
				var rigidbody = component.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					continue;
				}
				output += rigidbody.velocity;
			}
			return output / components.Count;
		}
        /// <summary>
        ///  rotation 映射的摄像机角度
        ///  offsetPos 为了方便调试和不让摄像机同时看到两个场景 我们给其中的一个场景加上位移 转回来的时候也相应的减去位移 
        /// </summary>
        private static Vector2 rotation = new Vector2(45, 0);
        public static Vector3 WorldToScreen(Vector3 worldPos,Vector3 offsetPos= default(Vector3))
        {
            worldPos += offsetPos;
            Vector3 ret = new Vector3(worldPos.x * Mathf.Cos(Mathf.Deg2Rad * rotation.y) + worldPos.y * Mathf.Sin(Mathf.Deg2Rad * rotation.y),
                worldPos.z * Mathf.Sin(Mathf.Deg2Rad * rotation.x) + worldPos.y * Mathf.Cos(Mathf.Deg2Rad * rotation.x), 0);
            return ret;
        }

        //屏幕坐标转换成游戏空间坐标，zGame:表示该物体在空间中的z
        public static Vector3 ScreenToWorld(Vector3 pos,Vector3 offsetPos= default(Vector3))
        {
            Vector2 screenPos = new Vector2(pos.x, pos.y);
            float zGame = pos.z;
            Vector3 ret = new Vector3((screenPos.x - zGame * Mathf.Sin(Mathf.Deg2Rad * rotation.y)) / Mathf.Cos(Mathf.Deg2Rad * rotation.y), 
                zGame, (screenPos.y - zGame * Mathf.Cos(Mathf.Deg2Rad * rotation.x)) / Mathf.Sin(Mathf.Deg2Rad * rotation.x));
            ret -= offsetPos;
            return ret;
        }
        public static void MappingAllChildren(GameObject mappingObj, MappingType selfMapping)
        {
            switch (selfMapping)
            {
                case MappingType.ScreenToWorld:
                    foreach (Transform point in GameObjectExtensions.GetComponentsInRealChildren<Transform>(mappingObj))
                    {
                        point.localPosition = VectorHelper.ScreenToWorld(point.localPosition);
                    }

                    break;
                case MappingType.WorldToScreen:
                    foreach (Transform point in GameObjectExtensions.GetComponentsInRealChildren<Transform>(mappingObj))
                    {
                        point.localPosition = VectorHelper.WorldToScreen(point.localPosition);
                    }
                    break;
                case MappingType.None:
                    break;
            }
        }
       
    }
}