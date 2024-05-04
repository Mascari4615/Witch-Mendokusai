namespace Mascari4615
{
	public class GameManager : Singleton<GameManager>
	{
		private void Start()
		{
			TimeManager.Instance.RegisterCallback(DataManager.Instance.WorkManager.TickEachWorks);
		}
	}
}