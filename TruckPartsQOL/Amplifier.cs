using UnityEngine;

namespace TruckPartsQOL
{
	// A 4-channel amp. Fitted in the same vehicle as the head unit (and the
	// vehicle can run accessories), it drives the speakers harder and gives the
	// subwoofer its full output; without one a sub only gets a weak signal.
	public class Amplifier : MonoBehaviour
	{
		public const float SpeakerGain = 1.35f;
		public const float SubGain = 1.6f;
		public const float SubWithoutAmp = 0.5f;

		public static bool In(GameObject vehicle)
		{
			if (vehicle == null)
			{
				return false;
			}
			foreach (TruckPart part in TruckPart.All)
			{
				if (part.kind == PartKind.Amplifier && part.installed && part.Vehicle == vehicle)
				{
					return true;
				}
			}
			return false;
		}
	}
}
