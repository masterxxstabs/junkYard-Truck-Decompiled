using System;
using System.Linq;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class Arena : MonoBehaviour
{
	// Token: 0x06000474 RID: 1140 RVA: 0x0002F5D4 File Offset: 0x0002D7D4
	private void Start()
	{
		this.CreateAICar(Color.red, 1);
		this.CreateAICar(Color.green, 2);
		this.CreateAICar(Color.yellow, 3);
		this.CreateAICar(Color.white, 4);
		this.CreateAICar(Color.magenta, 5);
		this.CreateAICar(Color.cyan, 6);
		this.CreateAICar(Color.gray, 7);
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0002F638 File Offset: 0x0002D838
	private void CreateAICar(Color color, int pos)
	{
		Car car = Object.Instantiate<Car>(this.CarModel.GetComponent<Car>());
		Object.Destroy(car.GetComponent<PlayerControl>());
		Object.Destroy(car.transform.Find("CarCam").gameObject);
		car.Color = color;
		car.transform.position = Quaternion.Euler(0f, (float)(pos * 45), 0f) * new Vector3(0f, 0f, -15f);
		car.transform.forward = -car.transform.position;
		car.transform.parent = base.transform;
		car.name = "AI Car";
		car.gameObject.AddComponent<AIControl>();
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0002F700 File Offset: 0x0002D900
	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Camera[] allCameras = Camera.allCameras;
			for (int i = 0; i < allCameras.Length; i++)
			{
				Ray ray = allCameras[i].ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit = default(RaycastHit);
				if (Physics.Raycast(ray, out raycastHit))
				{
					ImpactDeformable component = raycastHit.collider.GetComponent<ImpactDeformable>();
					if (component != null)
					{
						component.Repair(0.05f, new Vector3?(raycastHit.point), new float?(0.75f));
					}
				}
			}
		}
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0002F780 File Offset: 0x0002D980
	public void CreateCube()
	{
		Vector3 vector = Random.insideUnitCircle * 20f;
		vector.z = vector.y;
		vector.y = 15f;
		Object.Instantiate<GameObject>(this.BoxModel, vector, Random.rotation);
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0002F7CD File Offset: 0x0002D9CD
	public void RepairAll()
	{
		Object.FindObjectsOfType<ImpactDeformable>().ToList<ImpactDeformable>().ForEach(delegate(ImpactDeformable i)
		{
			i.Repair(1f, null, null);
		});
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0002F7FD File Offset: 0x0002D9FD
	public void RandomDamageAll()
	{
		Object.FindObjectsOfType<ImpactDeformable>().ToList<ImpactDeformable>().ForEach(delegate(ImpactDeformable i)
		{
			Collider component = i.GetComponent<Collider>();
			if (component == null)
			{
				return;
			}
			Vector3 vector = Random.onUnitSphere * 5f + i.transform.position;
			Ray ray = new Ray(vector, i.transform.position - vector);
			RaycastHit raycastHit;
			if (component.Raycast(ray, out raycastHit, 10f))
			{
				i.Deform(raycastHit.point, ray.direction.normalized * 0.3f);
			}
		});
	}

	// Token: 0x04000948 RID: 2376
	public GameObject CarModel;

	// Token: 0x04000949 RID: 2377
	public GameObject BoxModel;
}
