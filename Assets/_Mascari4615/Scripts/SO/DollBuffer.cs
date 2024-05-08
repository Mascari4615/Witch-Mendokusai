using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	[CreateAssetMenu(fileName = nameof(DollBuffer), menuName = "DataBuffer/" + nameof(Doll))]
	public class DollBuffer : DataBufferSO<Doll> { }
}