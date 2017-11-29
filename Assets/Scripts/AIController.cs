using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType {
	FIGHTER, FRIGATE, DESTROYER
}

public enum AIMode {
	IDLE, OFFENCE, DEFENCE, RETREAT
}

public class AIController : MonoBehaviour {

	public GameObject fighterPrefab;
	public GameObject frigatePrefab;
	public GameObject destroyerPrefab;

	private bool productionPrepared;
	private bool changedMode;
	private int playerBattleValue;
	private int battleValue;
	private float lastChangeModeTime;
	private float lastProductionTime;
	private float lastProductSize;
	private AIMode aiMode;
	private UnitType nextUnitType;

	private GameObject lastNewUnit;
	private GameObject station;
	private GameObject playerStation;
	private GameObject attackedUnit;
	private GameController gameController;
	private PiratesStationController stationController;

	// Use this for initialization
	void Start () {
		battleValue = 0;
		playerBattleValue = 0;
		lastChangeModeTime = 0.0f;
		lastProductionTime = 0.0f;
		lastProductSize = 0.0f;
		productionPrepared = false;
		changedMode = false;
		aiMode = AIMode.IDLE;
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		playerStation = GameObject.FindGameObjectWithTag("SpaceStation");
		station = GameObject.FindGameObjectWithTag("EnemyStation");
		stationController = station.GetComponent<PiratesStationController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (station != null) {
			if (productionPrepared == false) {
				PrepareProduction();
				productionPrepared = true;
			} else if (Time.time - lastProductionTime > lastProductSize) {
				Production();
			}
		}

		if (Time.time - lastChangeModeTime < 1.0f) {
			return;
		}

		if ((aiMode == AIMode.OFFENCE || aiMode == AIMode.DEFENCE) && stationController.isUnderAttack) {
			lastChangeModeTime = Time.time;
			changedMode = true;
			aiMode = AIMode.RETREAT;
			//RetreatAction();
			return;
		}

		if (aiMode == AIMode.IDLE) {
			if (battleValue >= 10 && battleValue >= 1.2f * playerBattleValue) {
				lastChangeModeTime = Time.time;
				changedMode = true;
				aiMode = AIMode.OFFENCE;
				//OffenceAction();
			} else {
				attackedUnit = whoIsAttacked();
				if (attackedUnit != null) {
					lastChangeModeTime = Time.time;
					changedMode = true;
					aiMode = AIMode.DEFENCE;
					//DefenceAction(attackedUnit.transform.position);
				}
			}
		} else if (aiMode == AIMode.OFFENCE) {
			//OffenceAction();
			if (battleValue <= 0.8f * playerBattleValue) {
				lastChangeModeTime = Time.time;
				changedMode = true;
				aiMode = AIMode.RETREAT;
				//RetreatAction();
			}
		} else if (aiMode == AIMode.RETREAT) {
			if (isAnyoneMoving() == false) {
				lastChangeModeTime = Time.time;
				changedMode = true;
				aiMode = AIMode.IDLE;
			}
		} else if (aiMode == AIMode.DEFENCE) {
			if (whoIsAttacked() == null) {
				lastChangeModeTime = Time.time;
				changedMode = true;
				aiMode = AIMode.RETREAT;
				//RetreatAction();
			}
		}
	}

	void FixedUpdate () {
		//Debug.Log("Player: " + playerBattleValue + "   Pirates: " + battleValue);
		if (changedMode == false) {
			return;
		} else {
			changedMode = false;
		}

		if (aiMode == AIMode.RETREAT) {
			RetreatAction();
			return;
		}
		if (aiMode == AIMode.OFFENCE) {
			OffenceAction();
			return;
		}
		if (aiMode == AIMode.DEFENCE) {
			DefenceAction();
			return;
		}
	}
		
	void OffenceAction() {
		gameController.enemyUnits.ForEach(unit => {
			if (unit != null) {
				unit.GetComponent<PiratesFighterController>().CalculateMovement(playerStation.transform.position);
			}
		});
	}

	void RetreatAction() {
		gameController.enemyUnits.ForEach(unit => {
			if (unit != null) {
				unit.GetComponent<PiratesFighterController>().CalculateMovement(station.transform.position);
			}
		});
	}

	void DefenceAction() {
		gameController.enemyUnits.ForEach(unit => {
			if (unit != null) {
				unit.GetComponent<PiratesFighterController>().CalculateMovement(attackedUnit.transform.position);
			}
		});
	}

	GameObject whoIsAttacked() {
		GameObject attackedUnit = null;
		gameController.enemyUnits.ForEach(unit => {
			if(unit != null && unit.GetComponent<PiratesFighterController>().isUnderAttack) {
				attackedUnit = unit;
			}
		});
		return attackedUnit;
	}

	bool isAnyoneMoving() {
		bool returnValue = false;
		gameController.enemyUnits.ForEach(unit => {
			if(unit.GetComponent<PiratesFighterController>().isMoving) {
				returnValue = true;
			}
		});
		return returnValue;
	}

	void PrepareProduction() {
		float randomNumber = Random.value;
		if (randomNumber < 0.6f) {
			nextUnitType = UnitType.FIGHTER;
		} else if (randomNumber < 0.98f) {
			nextUnitType = UnitType.FRIGATE;
		} else {
			nextUnitType = UnitType.DESTROYER;
		}
	}

	void Production() {
		if (lastNewUnit != null) {
			Vector3 position = new Vector3(station.transform.position.x + 2.0f, station.transform.position.y + 2.0f, station.transform.position.z);
			lastNewUnit.GetComponent<PiratesFighterController>().CalculateMovement(position);
		}

		if (nextUnitType == UnitType.FIGHTER) {
			if (stationController.Ore >= 5.0f) {
				lastNewUnit = GameObject.Instantiate(fighterPrefab, station.transform.position, Quaternion.identity);
				gameController.enemyUnits.Add(lastNewUnit);

				stationController.Ore -= 5.0f;
				lastProductionTime = Time.time;
				lastProductSize = lastNewUnit.GetComponent<Attributes>().battleValue;
				productionPrepared = false;
			}
		} else if (nextUnitType == UnitType.FRIGATE) {
			if (stationController.Ore >= 10.0f) {
				lastNewUnit = GameObject.Instantiate(frigatePrefab, station.transform.position, Quaternion.identity);
				gameController.enemyUnits.Add(lastNewUnit);

				stationController.Ore -= 10.0f;
				lastProductionTime = Time.time;
				lastProductSize = lastNewUnit.GetComponent<Attributes>().battleValue;
				productionPrepared = false;
			}
		} else if (nextUnitType == UnitType.DESTROYER) {
			if (stationController.Ore >= 30.0f) {
				lastNewUnit = GameObject.Instantiate(destroyerPrefab, station.transform.position, Quaternion.identity);
				gameController.enemyUnits.Add(lastNewUnit);

				stationController.Ore -= 30.0f;
				lastProductionTime = Time.time;
				lastProductSize = lastNewUnit.GetComponent<Attributes>().battleValue;
				productionPrepared = false;
			}
		}
	}

	public void IncreaseBattleValue(int value) {
		battleValue += value;
	}

	public void IncreasePlayerBattleValue(int value) {
		playerBattleValue += value;
	}

	public void DecreaseBattleValue(int value) {
		battleValue -= value;
	}

	public void DecreasePlayerBattleValue(int value) {
		playerBattleValue -= value;
	}
}
