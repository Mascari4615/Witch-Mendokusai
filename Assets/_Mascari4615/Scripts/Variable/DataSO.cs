using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mascari4615
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PropertyOrderAttribute : Attribute
	{
		public int Order { get; set; }

		public PropertyOrderAttribute(int order)
		{
			Order = order;
		}
	}
	
	public abstract class DataSO : ScriptableObject
	{
		public const int NONE_ID = -1;

		[field: Header("_" + nameof(DataSO))]
		
		[PropertyOrder(-100)] [field: SerializeField] public int ID { get; set; }
		[PropertyOrder(-99)] [field: SerializeField] public string Name { get; set; }
		[PropertyOrder(-98)] [field: SerializeField, TextArea] public string Description { get; set; }
		[PropertyOrder(-97)] [field: SerializeField] public Sprite Sprite { get; set; }
	}

	[System.Serializable]
	public struct DataSOWithPercentage
	{
		[field: Header("_" + nameof(DataSOWithPercentage))]
		[field: SerializeField] public DataSO DataSO { get; set; }
		[field: SerializeField] public float Percentage { get; set; }
	}
}