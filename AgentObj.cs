using UnityEngine;
using System.Collections;

public class AgentObj {

	public Vector3 levelSize;
	public int seedLayers;
	public Vector3 agentArrayPosition;
	public bool holdingCube = false;
	public CubeObj cubeHeld;
	public int agentLife;
	public GameObject agentGameObj;
	public CubeObj[,,] cubeNeighborhood;

	public AgentObj(Vector3 levelSize, int seedLayers, int agentLife){
		this.levelSize = levelSize;
		this.seedLayers = seedLayers;
		this.agentLife = agentLife;

		agentArrayPosition = new Vector3(Random.Range(0,(int)levelSize.x), Random.Range (0, (int)seedLayers), Random.Range(0, (int)levelSize.z));
	}
}
