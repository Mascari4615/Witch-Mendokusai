using UnityEngine;

namespace Mascari4615
{
	public class GameManager : Singleton<GameManager>
	{
		public bool IsChatting { get; set; }
		public bool IsDashing { get; set; }
		public bool IsCooling { get; set; }
		public bool IsDied { get; set; }
		public bool IsMouseOnUI { get; set; }
	}
}