using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Architect))]
[RequireComponent(typeof(TerrainManager))]

public class SceneBuilder : MonoBehaviour {
	
	public int cubeSize;
	public Vector3 levelSize = new Vector3(10f, 10f, 10f);
	public enum ModelType {
		LASIUS_NIGER
	}
	public ModelType modelType;
	public int desiredNetworkSize = 0;

	public int seedLayers = 2;
	public int targetBuildSteps;

	protected float cubeSpacing;
	protected bool levelLocked = false;
	protected bool cubeSeed = false;
	protected bool agentSeed = false;
	protected bool terrainSeed = false;
	protected bool networkCheck = false;
	protected int levelDimension;
	protected bool stillChecking = true;
	protected bool pathFind = false;

	protected CubeObj networkCube;

	Architect architect;
	TerrainManager terrainManager;
	AgentHandler agentHandler;

	List<GameObject> closedList = new List<GameObject>();
	List<GameObject> openList = new List<GameObject>();
	List<GameObject> hitList = new List<GameObject>();

	List<Ray> drawRays = new List<Ray>();
	
	void Awake(){
		architect = GetComponent<Architect>();
		terrainManager = GetComponent<TerrainManager>();
		agentHandler = GetComponent<AgentHandler>();
		cubeSpacing = (1 + cubeSize * .05f);
	}

	void Update(){
		foreach(Ray ray in drawRays){
			Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.magenta, Mathf.Infinity, false);
		}
	}

	void OnGUI(){

		if(cubeSeed == false) GUI.enabled = true;
		else GUI.enabled = false;

		if(GUI.Button(new Rect(10, 10, 150, 30), "Seed Cubes")){
			SeedCubes();
			cubeSeed = true;
		}

		if(cubeSeed == true && agentSeed == false) GUI.enabled = true;
		else GUI.enabled = false;

		if(GUI.Button (new Rect(10, 50, 150, 30), "Seed Agents")){
			agentSeed = true;
			agentHandler.SeedAgents();
		}

		if(agentSeed == true && levelLocked == false) GUI.enabled = true;
		else GUI.enabled = false;


		if(GUI.Button(new Rect(10, 90, 150, 30), "Use This Build")){
			levelLocked = true;
			LockBuild();
		}

		/*
		if(levelLocked == true && networkCheck == false) GUI.enabled = true;
		else GUI.enabled = false;

		if(GUI.Button(new Rect(10, 130, 150, 30), "Network Check")){
			networkCheck = true;
			NetworkCheck();
		}

		if(networkCheck == true) GUI.enabled = true;
		else GUI.enabled = false;

		if(GUI.Button(new Rect(10, 170, 150, 30), "Pathfind")){
			pathFind = true;
			PathFind();
		}
		*/

		GUI.enabled = true;

		GUI.Label(new Rect(10, 205, 150, 20), "Cube Size: " +cubeSize);
		cubeSize = (int)Mathf.Floor(GUI.HorizontalSlider(new Rect(10, 230, 150, 30), cubeSize, 2f, 32f));

		GUI.Label(new Rect(10, 325, 150, 20), "Step count: " +agentHandler.stepCount.ToString());
		GUI.Label(new Rect(10, 350, 150, 20), "Pickup count: " +agentHandler.cubeHeldCount.ToString());
		}

	void SeedCubes(){
		int[,,] cubePositionArray = terrainManager.cubePositionArray;
		int cubeNumber = 0;

		
		// Seed cube position array and generate seed cubes in scene
		// NOTE: y is set to break the loop at seedLayers, seeding only those y coords up to the user-specified seedLayers public var in this script.
		
		for(int y = 0; y < seedLayers; y++){
			for(int z = 0; z < levelSize.z; z++){
				for(int x = 0; x < levelSize.x; x++){
					cubePositionArray[x,y,z] = 1;
					architect.GenerateCube(cubeNumber, cubeSize, new Vector3(x, y, z), true);
					cubeNumber++;
				}
			}
		}
	}

	void LockBuild(){

		// Kill all agents

		agentHandler.KillAgents();

		// Eliminate floor cubes (i.e. all cubes with y = 0 in the cube array)

		List<CubeObj> cubeTrashList = terrainManager.cubeMasterList.Where (c => c.cubeArrayPosition.y == 0).ToList();
		foreach(CubeObj cube in cubeTrashList){
			Destroy(cube.cubeGameObj);
			terrainManager.cubeMasterList.Remove(cube);
		}

		// Push cubes away from one another by cubeSpacing in all dimensions

		List<GameObject> cubeList = terrainManager.cubeMasterList.Select(c => c.cubeGameObj).ToList();
		foreach(GameObject cube in cubeList){
			Vector3 originalPosition = cube.transform.position;
			Vector3 pushVector = new Vector3(cubeSpacing, cubeSpacing, cubeSpacing);
			Vector3 newPosition = Vector3.Scale(originalPosition, pushVector);

			// Augment y coord to produce level with more verticality

			int verticalStep = cubeSize + (int)pushVector.y;
			int verticalRand = (int)Random.Range(0, levelSize.y);
			newPosition = newPosition + new Vector3(0, verticalStep * verticalRand, 0);
			cube.transform.position = newPosition;
		}
	}
	
	void NetworkCheck(){

		List<GameObject> largestClosedList = new List<GameObject>();

		foreach(CubeObj seed in terrainManager.cubeMasterList){

			bool continueCasting = true;

			do {
				if(!openList.Any()) openList.Add(seed.cubeGameObj);

				foreach(GameObject cube in openList){
					Transform cubeTransform = cube.transform;
					
					RaycastHit cast1;
					RaycastHit cast2;
					RaycastHit cast3;
					RaycastHit cast4;
					RaycastHit cast5;
					RaycastHit cast6;
					
					if(Physics.Raycast(cubeTransform.position, cubeTransform.up, out cast1, 1000f, terrainManager.cubeFaceLayerMask)){
						if(!closedList.Any(c => c == cast1.collider.gameObject) && !openList.Any(c => c == cast1.collider.gameObject)) hitList.Add (cast1.collider.gameObject);
					}
					if(Physics.Raycast(cubeTransform.position, -cubeTransform.up, out cast2, 1000f, terrainManager.cubeFaceLayerMask)){
						if(!closedList.Any(c => c == cast2.collider.gameObject) && !openList.Any(c => c == cast2.collider.gameObject)) hitList.Add (cast2.collider.gameObject);
					}
					if(Physics.Raycast(cubeTransform.position, cubeTransform.right, out cast3, 1000f, terrainManager.cubeFaceLayerMask)){
						if(!closedList.Any(c => c == cast3.collider.gameObject) && !openList.Any(c => c == cast3.collider.gameObject)) hitList.Add (cast3.collider.gameObject);
					}
					if(Physics.Raycast(cubeTransform.position, -cubeTransform.right, out cast4, 1000f, terrainManager.cubeFaceLayerMask)){
						if(!closedList.Any(c => c == cast4.collider.gameObject) && !openList.Any(c => c == cast4.collider.gameObject)) hitList.Add (cast4.collider.gameObject);
					}
					if(Physics.Raycast(cubeTransform.position, cubeTransform.forward, out cast5, 1000f, terrainManager.cubeFaceLayerMask)){
						if(!closedList.Any(c => c == cast5.collider.gameObject) && !openList.Any(c => c == cast5.collider.gameObject)) hitList.Add (cast5.collider.gameObject);
					}
					if(Physics.Raycast(cubeTransform.position, -cubeTransform.forward, out cast6, 1000f, terrainManager.cubeFaceLayerMask)){
						if(!closedList.Any(c => c == cast6.collider.gameObject) && !openList.Any(c => c == cast6.collider.gameObject)) hitList.Add (cast6.collider.gameObject);
					}
				}

				if(hitList.Any ()) {
					foreach(GameObject toClosed in openList) closedList.Add (toClosed);
					openList.Clear();

					foreach(GameObject toOpen in hitList) openList.Add(toOpen);
					hitList.Clear();

				} else {
					foreach(GameObject toClosed in openList) closedList.Add (toClosed);
					openList.Clear();

					seed.networkSize = closedList.Count();
					if(closedList.Count() > largestClosedList.Count()){
						largestClosedList.Clear();
						foreach(GameObject c in closedList){
							largestClosedList.Add(c);
						}
					}
					closedList.Clear();
					continueCasting = false;
				}
			}
			while (continueCasting == true);
		}

		// Find the cube with the largest networkSize and SHOW VISUAL RAYCASTS FOR DEBUG

		networkCube = terrainManager.cubeMasterList.Aggregate((c1,c2) => c1.networkSize > c2.networkSize ? c1 : c2);
		if (networkCube != null) Debug.Log ("We have a max network cube!");

		bool debugCast = true;
		int iteration = 0;

		do {

			Debug.Log ("This is iteration: " +iteration);
			if(!openList.Any()) openList.Add(networkCube.cubeGameObj);
			
			foreach(GameObject cube in openList){
				Transform cubeTransform = cube.transform;
				
				RaycastHit cast1;
				RaycastHit cast2;
				RaycastHit cast3;
				RaycastHit cast4;
				RaycastHit cast5;
				RaycastHit cast6;

				Ray ray1 = new Ray(cubeTransform.position, cubeTransform.up);
				Ray ray2 = new Ray(cubeTransform.position, -cubeTransform.up);
				Ray ray3 = new Ray(cubeTransform.position, cubeTransform.right);
				Ray ray4 = new Ray(cubeTransform.position, -cubeTransform.right);
				Ray ray5 = new Ray(cubeTransform.position, cubeTransform.forward);
				Ray ray6 = new Ray(cubeTransform.position, -cubeTransform.forward);

				drawRays.Add(ray1);
				drawRays.Add(ray2);
				drawRays.Add(ray3);
				drawRays.Add(ray4);
				drawRays.Add(ray5);
				drawRays.Add(ray6);

				
				if(Physics.Raycast(ray1, out cast1, 1000f, terrainManager.cubeFaceLayerMask)){
					if(!closedList.Any(c => c == cast1.collider.gameObject) && !openList.Any(c => c == cast1.collider.gameObject)) hitList.Add (cast1.collider.gameObject);
				}
				if(Physics.Raycast(ray2, out cast2, 1000f, terrainManager.cubeFaceLayerMask)){
					if(!closedList.Any(c => c == cast2.collider.gameObject) && !openList.Any(c => c == cast2.collider.gameObject)) hitList.Add (cast2.collider.gameObject);
				}
				if(Physics.Raycast(ray3, out cast3, 1000f, terrainManager.cubeFaceLayerMask)){
					if(!closedList.Any(c => c == cast3.collider.gameObject) && !openList.Any(c => c == cast3.collider.gameObject)) hitList.Add (cast3.collider.gameObject);
				}
				if(Physics.Raycast(ray4, out cast4, 1000f, terrainManager.cubeFaceLayerMask)){
					if(!closedList.Any(c => c == cast4.collider.gameObject) && !openList.Any(c => c == cast4.collider.gameObject)) hitList.Add (cast4.collider.gameObject);
				}
				if(Physics.Raycast(ray5, out cast5, 1000f, terrainManager.cubeFaceLayerMask)){
					if(!closedList.Any(c => c == cast5.collider.gameObject) && !openList.Any(c => c == cast5.collider.gameObject)) hitList.Add (cast5.collider.gameObject);
				}
				if(Physics.Raycast(ray6, out cast6, 1000f, terrainManager.cubeFaceLayerMask)){
					if(!closedList.Any(c => c == cast6.collider.gameObject) && !openList.Any(c => c == cast6.collider.gameObject)) hitList.Add (cast6.collider.gameObject);
				}
			}
			
			if(hitList.Any ()) {
				foreach(GameObject toClosed in openList) closedList.Add (toClosed);
				openList.Clear();
				
				foreach(GameObject toOpen in hitList) openList.Add(toOpen);
				hitList.Clear();
				
			} else {
				foreach(GameObject toClosed in openList) closedList.Add (toClosed);
				openList.Clear();
				debugCast = false;
			}
			iteration++;
		}
		while (debugCast == true);

		foreach(GameObject networkedCube in largestClosedList){
			Renderer[] faces = GetComponentsInChildren<Renderer>();
			foreach(Renderer face in faces){
				face.material.color = Color.cyan;
			}
		}

		// Mark the networkCube in red

		foreach(FaceObj face in networkCube.faceList){
			face.faceGameObj.renderer.material.color = Color.red;
		}

		// Clear cubeObjs not part of the longestClosedList network from the cubeMasterList and their cubeGameObj from the Scene

		List<CubeObj> destroyList = new List<CubeObj>();

		foreach(CubeObj checkCube in terrainManager.cubeMasterList){
			GameObject destroyCandidate = checkCube.cubeGameObj;
			Debug.Log ("Largest closed list has length: " +largestClosedList.Count());
			if(!largestClosedList.Any(c => c.GetInstanceID() == destroyCandidate.GetInstanceID())){
				Destroy(destroyCandidate);
				destroyList.Add(checkCube);
			}
		}

		foreach(CubeObj destroyCube in destroyList){
			terrainManager.cubeMasterList.Remove(destroyCube);
		}

		/*
		foreach(CubeObj cube in terrainManager.cubeMasterList){

			Transform cubeTransform = cube.cubeGameObj.transform;
			Vector3 cubePosition = cubeTransform.position;

			bool cast1 = Physics.Raycast(cubePosition, cubeTransform.up, terrainManager.cubeFaceLayerMask);
			bool cast2 = Physics.Raycast(cubePosition, -cubeTransform.up, terrainManager.cubeFaceLayerMask);
			bool cast3 = Physics.Raycast(cubePosition, cubeTransform.right, terrainManager.cubeFaceLayerMask);
			bool cast4 = Physics.Raycast(cubePosition, -cubeTransform.right, terrainManager.cubeFaceLayerMask);
			bool cast5 = Physics.Raycast(cubePosition, cubeTransform.forward, terrainManager.cubeFaceLayerMask);
			bool cast6 = Physics.Raycast(cubePosition, -cubeTransform.forward, terrainManager.cubeFaceLayerMask);

			if(!cast1 && !cast2 && !cast3 && !cast4 && !cast5 && !cast6){
				foreach(FaceObj face in cube.faceList){
					face.faceGameObj.renderer.material.color = Color.red;
					Destroy(cube.cubeGameObj);
				}
			} else {
				foreach(FaceObj face in cube.faceList){
					face.faceGameObj.renderer.material.color = Color.green;
				}

			}
		}
		*/
	}

	void PathFind(){

		// Calculate positionSum for each cube
		
		foreach(CubeObj cube in terrainManager.cubeMasterList){
			cube.PositionSum();
		}
		
		// Determine start cube, end cube, check for accessibility, and set their material colors to mark them in the Scene
		
		CubeObj startCube = terrainManager.cubeMasterList.Aggregate((c1, c2) => c1.positionSum > c2.positionSum ? c1 : c2);
		CubeObj endCube = terrainManager.cubeMasterList.Aggregate((c1,c2) => c1.positionSum < c2.positionSum ? c1 : c2);
		
		foreach(FaceObj face in startCube.faceList){
			face.faceGameObj.renderer.material.color = Color.blue;
		}
		
		foreach(FaceObj face in endCube.faceList){
			face.faceGameObj.renderer.material.color = Color.yellow;
		}



	}

	void SeedTerrain(){
		

		
		// Seed each cube with rooms



	}
}
