using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AgentHandler : MonoBehaviour {

	public int agentCount = 20;
	public int agentLife = 100;
	public float evap = 0.0032f;
	public int stepCount = 0;
	public int cubeHeldCount = 0;
	public float pickupModifier = 10f; // Modifies the thereshold chance for pickup. Higher modifier = greater chance of pickup
	public float dropModifier = 10f; // Modifes the threshold chance for drop. Higher modifier = greater chance of drop

	protected List<AgentObj> agentList = new List<AgentObj>();

	Architect architect;
	TerrainManager terrainManager;
	SceneBuilder sceneBuilder;

	void Awake(){
		architect = GetComponent<Architect>();
		terrainManager = GetComponent<TerrainManager>();
		sceneBuilder = GetComponent<SceneBuilder>();
	}

	public void SeedAgents(){
		for(int agent = 0; agent < agentCount; agent++){
			AgentObj newAgent = new AgentObj(sceneBuilder.levelSize, sceneBuilder.seedLayers, agentLife);
			agentList.Add(newAgent);
			newAgent.cubeNeighborhood = architect.NeighborCheck(newAgent.agentArrayPosition);
		}
	}

	void Update(){ 
		if(agentList.Count != 0){
			foreach(AgentObj agent in agentList){
				if(!agent.holdingCube) PickUp (agent);
				RandomWalk(agent);
				if(agent.holdingCube) Drop (agent);
				//agent.agentLife--;
			}
			stepCount++;
		}
	}

	void PickUp(AgentObj agent){

		CubeObj[,,] neighborhood = agent.cubeNeighborhood;
		float spontPick = 0.1f;
		float amplifPick = 1.0f;

		// Pickup targets are always 1 y coord below the worker
		// If the pickup candidate cell is full and is not already flagged as picked up by another worker, then run stochastic check for pickup.
		// If the roll is successful, then pick up.

		if(neighborhood[1, 0, 1] != null && neighborhood[1,0,1].heldByAgent == false){
			//Pickup targets are only influenced by the eight adjacent neighbors that share the same y coord
			int neighborCount = 0;

			for(int x = 0; x < neighborhood.GetLength(0); x++){
				for(int z = 0; z < neighborhood.GetLength(2); z++){
					if(neighborhood[x, 0, z] != null && !(x == 1 && z == 1)){
						neighborCount++;
					}
				}
			}
			float pickupChance = spontPick / (amplifPick * neighborCount);
			float pickupRoll = Random.Range(0.000f,1.000f);
			pickupChance = pickupChance * pickupModifier;

			if (pickupRoll < pickupChance){
				CubeObj pickupTarget = neighborhood[1, 0, 1];
				agent.holdingCube = true;
				agent.cubeHeld = pickupTarget;
				pickupTarget.heldByAgent = true;
				cubeHeldCount++;
			}
		}
	}

	void RandomWalk(AgentObj agent){

		List<Vector3> viableMoveList = new List<Vector3>();

		CubeObj[,,] neighborhood = agent.cubeNeighborhood;

		for(int x = 0; x < neighborhood.GetLength(0); x++){
			for(int y = 0; y < neighborhood.GetLength(1); y++){
				for(int z = 0; z < neighborhood.GetLength(2); z++){
					if(neighborhood[x,y,z] != null){
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition);
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition + new Vector3(0,1,0));
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition + new Vector3(0,-1,0));
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition + new Vector3(1,0,0));
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition + new Vector3(-1,1,0));
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition + new Vector3(0,0,1));
						viableMoveList.Add(neighborhood[x,y,z].cubeArrayPosition + new Vector3(0,0,-1));
					}
				}
			}
		}

		// Scrub list of coordinates outside the level size

		viableMoveList.RemoveAll(coord => (
			coord.x >= sceneBuilder.levelSize.x || 
			coord.y >= sceneBuilder.levelSize.y || 
			coord.z >= sceneBuilder.levelSize.z ||
			coord.x < 0 ||
			coord.y < 0 ||
			coord.z < 0
			));

		//Select a move from the viable move list and update agent's cubeNeighborhood array

		Vector3 newPosition = viableMoveList[Random.Range(0, viableMoveList.Count)];
		agent.agentArrayPosition = newPosition;
		agent.cubeNeighborhood = architect.NeighborCheck(agent.agentArrayPosition);
	}

	void Drop(AgentObj agent){
		float spontDrop = 0.001f;
		float drop1 = 0.01f;
		float amplifDrop = 0.036f;
		float dropChance = 0f;
		float dropRoll = Random.Range(0.000f, 1.000f);

		if(!terrainManager.CheckForCube(agent.agentArrayPosition) && agent.holdingCube){

			// Find number of neighbors for current agent position

			CubeObj[,,] neighborhood = agent.cubeNeighborhood;
			int neighborCount = architect.CountNeighbors(neighborhood);

			// Find latest drop time in neighborhood

			float latestDropTime = terrainManager.LatestDropInNeighborhood(agent.agentArrayPosition);

			if(neighborCount == 0) dropChance = spontDrop;
			else {
				dropChance = (drop1 + amplifDrop * (neighborCount - 1)) * Mathf.Exp(-(Time.time - latestDropTime) * evap);
			}

			dropChance *= dropModifier;

			if(dropRoll < dropChance){
				CubeObj dropCube = agent.cubeHeld;
				Vector3 thisPosition = agent.agentArrayPosition;

				terrainManager.cubePositionArray[(int)dropCube.cubeArrayPosition.x, (int)dropCube.cubeArrayPosition.y, (int)dropCube.cubeArrayPosition.z] = 0;
				terrainManager.cubePositionArray[(int)thisPosition.x, (int)thisPosition.y, (int)thisPosition.z] = 1;

				dropCube.cubeArrayPosition = thisPosition;
				dropCube.cubeGameObj.transform.position = thisPosition * sceneBuilder.cubeSize;

				agent.holdingCube = false;
				dropCube.heldByAgent = false;
			}

		}
	}

	public void KillAgents(){
		agentList.Clear();
	}
}
