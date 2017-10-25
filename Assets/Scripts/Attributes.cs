using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes: MonoBehaviour {

	public float SizeX;
	public float SizeY;

	public int direction; // 0 - up, 1 - right, 2 - down, 3 - left

	public int owner; // 0 - player, others - enemies
	public int hp;
	public int damage;
	public float reloadTime;
}
